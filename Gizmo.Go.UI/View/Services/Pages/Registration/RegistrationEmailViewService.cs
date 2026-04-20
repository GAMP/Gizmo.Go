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
    [Route(NavigationHelper.RegistrationEmail)]
    public sealed class RegistrationEmailViewService : ViewStateServiceBase<RegistrationEmailViewState>
    {
        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;

        public RegistrationEmailViewService(
            RegistrationEmailViewState viewState,
            ILogger<RegistrationEmailViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
        }

        #endregion

        #region METHODS

        public ValueTask SetEmailAsync(string value)
        {
            ViewState.Email = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask SubmitAsync()
        {
            // TODO: wire email registration
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateBackAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateToAlternativeAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateToLoginAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.LoginPage);
            return ValueTask.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ViewState.Email = string.Empty;
            ViewState.RaiseChanged();
            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        #endregion
    }
}
