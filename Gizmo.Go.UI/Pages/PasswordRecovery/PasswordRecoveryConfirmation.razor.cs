using Gizmo.Go.UI.View.Services.Pages.PasswordRecovery;
using Gizmo.Go.UI.View.States.Pages.PasswordRecovery;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.PasswordRecovery
{
    public partial class PasswordRecoveryConfirmation : ComponentBase, IDisposable
    {
        [Inject]
        private PasswordRecoveryConfirmationViewState PasswordRecoveryConfirmationViewState { get; set; } = null!;

        [Inject]
        private PasswordRecoveryConfirmationViewService PasswordRecoveryConfirmationViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(PasswordRecoveryConfirmationViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(PasswordRecoveryConfirmationViewState);
        }

        private async Task ConfirmAsync() => await PasswordRecoveryConfirmationViewService.ConfirmAsync();

        private async Task ResendCode() => await PasswordRecoveryConfirmationViewService.RestartTimerAsync();

        private async Task OpenAsync() => await PasswordRecoveryConfirmationViewService.OpenAsync();

        private async Task NavigateBack() => await PasswordRecoveryConfirmationViewService.NavigateBackAsync();

        private async Task NavigateToDestination() => await PasswordRecoveryConfirmationViewService.NavigateToDestinationAsync();
    }
}
