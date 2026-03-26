using Gizmo.Go.UI.View.States;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages
{
    [Register()]
    [Route("/create-password")]
    public sealed class CreatePasswordViewService : ViewStateServiceBase<CreatePasswordViewState>
    {
        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;

        public CreatePasswordViewService(
            CreatePasswordViewState viewState,
            ILogger<CreatePasswordViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
        }

        #endregion

        #region METHODS

        public ValueTask SetPasswordAsync(string value)
        {
            ViewState.Password = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask SetConfirmAsync(string value)
        {
            ViewState.Confirm = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask SubmitAsync()
        {
            if (!ViewState.CanSubmit)
                return ValueTask.CompletedTask;

            _navigationService.NavigateTo("/home");
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateBackAsync()
        {
            _navigationService.NavigateTo("/create-account");
            return ValueTask.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ViewState.Password = string.Empty;
            ViewState.Confirm = string.Empty;
            ViewState.RaiseChanged();
            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        #endregion
    }
}
