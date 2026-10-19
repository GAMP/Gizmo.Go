using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationSuccessRoute)]
    public sealed class UserRegistrationSuccessViewService : ViewStateServiceBase<UserRegistrationSuccessViewState>
    {
        private readonly IRegistrationSessionService _registrationSession;

        public UserRegistrationSuccessViewService(
            UserRegistrationSuccessViewState viewState,
            ILogger<UserRegistrationSuccessViewService> logger,
            IServiceProvider serviceProvider,
            IRegistrationSessionService registrationSession) : base(viewState, logger, serviceProvider)
        {
            _registrationSession = registrationSession;
        }

        public Task NavigateToLoginAsync()
        {
            _registrationSession.Clear();
            NavigationService.NavigateTo(NavigationHelper.LoginPage);
            return Task.CompletedTask;
        }
    }
}
