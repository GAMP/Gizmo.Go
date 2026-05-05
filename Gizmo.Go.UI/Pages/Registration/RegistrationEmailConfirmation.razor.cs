using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationEmailConfirmation : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        private RegistrationEmailConfirmationViewState RegistrationEmailConfirmationViewState { get; set; } = null!;

        [Inject]
        private RegistrationEmailConfirmationViewService RegistrationEmailConfirmationViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationEmailConfirmationViewState);
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationEmailConfirmationViewState);
        }

        private async Task ConfirmAsync() => await RegistrationEmailConfirmationViewService.ConfirmAsync();

        private async Task NavigateBack() => await RegistrationEmailConfirmationViewService.NavigateBackAsync();

        private async Task RestartTimer() => await RegistrationEmailConfirmationViewService.RestartTimerAsync();

        #endregion
    }
}
