using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.PasswordRecovery;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services
{
    [Register()]
    [Route(NavigationHelper.WelcomePage)]
    public sealed class WelcomeViewService : ViewStateServiceBase<WelcomeViewState>
    {
        private readonly NavigationService _navigationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly IPasswordRecoverySessionService _passwordRecoverySession;

        public WelcomeViewService(
            WelcomeViewState viewState,
            ILogger<WelcomeViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IRegistrationSessionService registrationSession,
            IPasswordRecoverySessionService passwordRecoverySession) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _registrationSession = registrationSession;
            _passwordRecoverySession = passwordRecoverySession;
        }

        public ValueTask NavigateToLoginAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.LoginPage);
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateToRegisterAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationIndexRoute);
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Leaves the registration or password recovery flow from an error: clears both sessions
        /// (the step view services reset on Cleared / on navigation) and opens Welcome.
        /// </summary>
        public ValueTask LeaveAuthFlowAsync()
        {
            _registrationSession.Clear();
            _passwordRecoverySession.Clear();
            _navigationService.NavigateTo(NavigationHelper.WelcomePage);
            return ValueTask.CompletedTask;
        }
    }
}
