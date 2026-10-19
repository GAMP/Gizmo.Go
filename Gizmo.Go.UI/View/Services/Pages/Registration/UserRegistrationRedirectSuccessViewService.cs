using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Common;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationRedirectSuccessRoute)]
    public sealed class UserRegistrationRedirectSuccessViewService : ViewStateServiceBase<UserRegistrationRedirectSuccessViewState>
    {
        private readonly IRegistrationSessionService _registrationSession;

        public UserRegistrationRedirectSuccessViewService(
            UserRegistrationRedirectSuccessViewState viewState,
            ILogger<UserRegistrationRedirectSuccessViewService> logger,
            IServiceProvider serviceProvider,
            IRegistrationSessionService registrationSession) : base(viewState, logger, serviceProvider)
        {
            _registrationSession = registrationSession;
        }

        public Task ConfirmAsync()
        {
            NavigationService.NavigateTo(NavigationHelper.RegistrationBasicFieldsRoute);
            return Task.CompletedTask;
        }

        public Task NavigateBackAsync()
        {
            // the session is cleared (agreements, required info), so a new attempt starts from the Index step
            _registrationSession.Clear();
            NavigationService.NavigateTo(NavigationHelper.RegistrationIndexRoute);
            return Task.CompletedTask;
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_registrationSession.Token))
            {
                NavigationService.NavigateTo(NavigationHelper.RegistrationProvidersRoute);
                return Task.CompletedTask;
            }

            ViewState.DisplayPhone = string.IsNullOrEmpty(_registrationSession.ActualContact)
                ? string.Empty
                : ContactMasking.MaskPhone(_registrationSession.ActualContact);
            ViewState.RaiseChanged();

            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }
    }
}
