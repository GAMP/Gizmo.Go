using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.PasswordRecovery;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.UI.Services;

namespace Gizmo.Go.UI.Services.Common
{
    public sealed class AuthFlowService : IAuthFlowService
    {
        public AuthFlowService(
            IRegistrationSessionService registrationSession,
            IPasswordRecoverySessionService passwordRecoverySession,
            NavigationService navigationService)
        {
            _registrationSession = registrationSession;
            _passwordRecoverySession = passwordRecoverySession;
            _navigationService = navigationService;
        }

        private readonly IRegistrationSessionService _registrationSession;
        private readonly IPasswordRecoverySessionService _passwordRecoverySession;
        private readonly NavigationService _navigationService;

        public void Leave()
        {
            _registrationSession.Clear();
            _passwordRecoverySession.Clear();
            _navigationService.NavigateTo(NavigationHelper.WelcomePage);
        }
    }
}
