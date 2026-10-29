using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Common;
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
        private readonly IAuthOptionsService _authOptionsService;

        public WelcomeViewService(
            WelcomeViewState viewState,
            ILogger<WelcomeViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IAuthOptionsService authOptionsService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _authOptionsService = authOptionsService;
        }

        /// <summary>
        /// Loads the server options of the screen. Called by the page on every show (also on the app start, when the
        /// first navigation does not reach the view services); a failed load is retried on the next show.
        /// </summary>
        public async Task LoadOptionsAsync(CancellationToken cancellationToken = default)
        {
            var options = await _authOptionsService.GetAsync(cancellationToken);

            ViewState.HasOptionsError = options is null;
            ViewState.IsRegistrationEnabled = options?.IsRegistrationEnabled ?? false;
            ViewState.RaiseChanged();
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
    }
}
