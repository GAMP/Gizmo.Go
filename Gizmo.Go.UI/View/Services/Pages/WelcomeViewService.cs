using Gizmo.Go.UI.Helpers;
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

        public WelcomeViewService(
            WelcomeViewState viewState,
            ILogger<WelcomeViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
        }

        public ValueTask NavigateToLoginAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.LoginPage);
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateToRegisterAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
            return ValueTask.CompletedTask;
        }

        public ValueTask ContinueAsGuestAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.WelcomePage);
            return ValueTask.CompletedTask;
        }
    }
}
