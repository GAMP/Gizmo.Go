using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationConfirmation : ComponentBase, IDisposable
    {
        [Inject]
        private RegistrationConfirmationViewState RegistrationConfirmationViewState { get; set; } = null!;

        [Inject]
        private RegistrationConfirmationViewService RegistrationConfirmationViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationConfirmationViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationConfirmationViewState);
        }

        private async Task ConfirmAsync() => await RegistrationConfirmationViewService.ConfirmAsync();

        private async Task NavigateBack() => await RegistrationConfirmationViewService.NavigateBackAsync();

        private async Task RestartTimer() => await RegistrationConfirmationViewService.RestartTimerAsync();
    }
}
