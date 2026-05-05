using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationSuccess)]
    public sealed class RegistrationSuccessViewService : ViewStateServiceBase<RegistrationSuccessViewState>
    {
        private readonly NavigationService _navigationService;

        public RegistrationSuccessViewService(
            RegistrationSuccessViewState viewState,
            ILogger<RegistrationSuccessViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
        }

        public ValueTask NavigateHomeAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.WelcomePage);
            return ValueTask.CompletedTask;
        }
    }
}
