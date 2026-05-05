using System.Web;
using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationEmail)]
    public sealed class RegistrationEmailViewService : ValidatingViewStateServiceBase<RegistrationEmailViewState>
    {
        private readonly NavigationService _navigationService;
        private readonly IRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;

        public RegistrationEmailViewService(
            RegistrationEmailViewState viewState,
            ILogger<RegistrationEmailViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IRegistrationService registrationService,
            IRegistrationSessionService registrationSession) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _registrationService = registrationService;
            _registrationSession = registrationSession;
        }

        public ValueTask SetEmailAsync(string value)
        {
            ViewState.Email = value;
            ValidateProperty(() => ViewState.Email);
            return ValueTask.CompletedTask;
        }

        public async ValueTask SubmitAsync()
        {
            Validate();
            if (ViewState.IsValid != true || ViewState.IsSubmitting)
                return;

            ViewState.IsSubmitting = true;
            ViewState.RaiseChanged();

            RegistrationStartResult result;
            try
            {
                result = await _registrationService.StartAsync(new RegistrationStartRequest
                {
                    IntegrationPublicId = ViewState.IntegrationPublicId,
                    DeliveryMethod = RegistrationDeliveryMethod.CodeDispatch,
                    Email = ViewState.Email
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to start email registration for provider {IntegrationPublicId}.", ViewState.IntegrationPublicId);
                ViewState.IsSubmitting = false;
                _navigationService.NavigateTo(NavigationHelper.RegistrationProviders + "?error=1");
                return;
            }

            if (result.Result != RegistrationStartResultCode.Success || string.IsNullOrEmpty(result.Token))
            {
                Logger.LogWarning("Email registration start failed with result {Result}.", result.Result);
                ViewState.IsSubmitting = false;
                _navigationService.NavigateTo(NavigationHelper.RegistrationProviders + "?error=1");
                return;
            }

            _registrationSession.SetToken(result.Token);
            _registrationSession.SetCodeLength(result.CodeLength);
            _registrationSession.SetEmail(ViewState.Email);
            _registrationSession.SetFlow(RegistrationFlow.Email);
            _navigationService.NavigateTo(NavigationHelper.RegistrationEmailConfirmation);
        }

        public ValueTask NavigateBackAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateToAlternativeAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateToLoginAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.LoginPage);
            return ValueTask.CompletedTask;
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            var uri = new Uri(_navigationService.GetUri());
            var raw = HttpUtility.ParseQueryString(uri.Query).Get("provider");

            if (!Guid.TryParse(raw, out var providerId))
            {
                _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
                return Task.CompletedTask;
            }

            ViewState.IntegrationPublicId = providerId;
            ViewState.Email = string.Empty;
            ViewState.IsSubmitting = false;
            ViewState.RaiseChanged();

            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }
    }
}
