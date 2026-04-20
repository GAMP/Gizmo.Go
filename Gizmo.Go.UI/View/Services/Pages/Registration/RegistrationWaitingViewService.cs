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
    [Route(NavigationHelper.RegistrationWaiting)]
    public sealed class RegistrationWaitingViewService : ViewStateServiceBase<RegistrationWaitingViewState>
    {
        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;

        public RegistrationWaitingViewService(
            RegistrationWaitingViewState viewState,
            ILogger<RegistrationWaitingViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
        }

        #endregion

        #region METHODS

        public ValueTask NavigateBackAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
            return ValueTask.CompletedTask;
        }

        #endregion
    }
}
