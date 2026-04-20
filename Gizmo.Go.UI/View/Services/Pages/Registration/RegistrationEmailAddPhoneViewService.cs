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
    [Route(NavigationHelper.RegistrationEmailAddPhone)]
    public sealed class RegistrationEmailAddPhoneViewService : ViewStateServiceBase<RegistrationEmailAddPhoneViewState>
    {
        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;

        public RegistrationEmailAddPhoneViewService(
            RegistrationEmailAddPhoneViewState viewState,
            ILogger<RegistrationEmailAddPhoneViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
        }

        #endregion

        #region METHODS

        public ValueTask SetPhoneAsync(string value)
        {
            ViewState.Phone = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask SubmitAsync()
        {
            // TODO: wire phone submission
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateBackAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationEmailConfirmation);
            return ValueTask.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ViewState.Phone = string.Empty;
            ViewState.ConfirmedEmail = string.Empty;
            ViewState.RaiseChanged();
            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        #endregion
    }
}
