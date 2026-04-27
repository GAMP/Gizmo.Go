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
    [Route(NavigationHelper.RegistrationBotVerifySuccess)]
    public sealed class RegistrationBotVerifySuccessViewService : ViewStateServiceBase<RegistrationBotVerifySuccessViewState>
    {
        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly IPhoneValidationService _phoneValidationService;

        public RegistrationBotVerifySuccessViewService(
            RegistrationBotVerifySuccessViewState viewState,
            ILogger<RegistrationBotVerifySuccessViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IRegistrationSessionService registrationSession,
            IPhoneValidationService phoneValidationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _registrationSession = registrationSession;
            _phoneValidationService = phoneValidationService;
        }

        #endregion

        #region METHODS

        public ValueTask ConfirmAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationPasswordPage);
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateBackAsync()
        {
            _registrationSession.Clear();
            _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
            return ValueTask.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (!_registrationSession.HasToken)
            {
                _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
                return Task.CompletedTask;
            }

            ViewState.DisplayPhone = _phoneValidationService.MaskPhone(_registrationSession.Phone);
            ViewState.RaiseChanged();

            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        #endregion
    }
}
