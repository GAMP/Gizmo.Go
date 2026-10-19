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
    [Route(NavigationHelper.RegistrationErrorRoute)]
    public sealed class UserRegistrationErrorViewService : ViewStateServiceBase<UserRegistrationErrorViewState>
    {
        private readonly IRegistrationSessionService _registrationSession;

        public UserRegistrationErrorViewService(
            UserRegistrationErrorViewState viewState,
            ILogger<UserRegistrationErrorViewService> logger,
            IServiceProvider serviceProvider,
            IRegistrationSessionService registrationSession) : base(viewState, logger, serviceProvider)
        {
            _registrationSession = registrationSession;
        }

        public Task GoBackAsync()
        {
            _registrationSession.SetShowAllProviders(true);
            NavigationService.NavigateTo(NavigationHelper.RegistrationProvidersRoute);
            return Task.CompletedTask;
        }

        public Task TryAgainAsync()
        {
            NavigationService.NavigateTo(NavigationHelper.RegistrationRedirectRoute);
            return Task.CompletedTask;
        }

        public Task NavigateToLoginAsync()
        {
            NavigationService.NavigateTo(NavigationHelper.LoginPage);
            return Task.CompletedTask;
        }
    }
}
