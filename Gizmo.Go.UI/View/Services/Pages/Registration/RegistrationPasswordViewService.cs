using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationPasswordPage)]
    public sealed class RegistrationPasswordViewService : ViewStateServiceBase<RegistrationPasswordViewState>
    {
        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;
        private readonly IRegistrationSessionService _registrationSession;

        public RegistrationPasswordViewService(
            RegistrationPasswordViewState viewState,
            ILogger<RegistrationPasswordViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IRegistrationSessionService registrationSession) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _registrationSession = registrationSession;
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
            if (!ViewState.CanSubmit || ViewState.IsSubmitting)
                return ValueTask.CompletedTask;

            ViewState.IsSubmitting = true;
            ViewState.RaiseChanged();

            _registrationSession.SetPassword(ViewState.Password);
            _navigationService.NavigateTo(NavigationHelper.RegistrationProfilePage);

            ViewState.IsSubmitting = false;
            ViewState.RaiseChanged();

            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateBackAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationPhone);
            return ValueTask.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (!_registrationSession.HasToken)
            {
                _navigationService.NavigateTo(NavigationHelper.RegistrationPhone);
                return base.OnNavigatedIn(navigationParameters, cancellationToken);
            }

            ViewState.IsSubmitting = false;
            ViewState.Password = string.Empty;
            ViewState.Confirm = string.Empty;
            ViewState.RaiseChanged();

            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        #endregion
    }
}
