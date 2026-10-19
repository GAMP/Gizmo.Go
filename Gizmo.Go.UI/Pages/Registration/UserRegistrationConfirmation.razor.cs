using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationConfirmation : ComponentBase, IDisposable
    {
        [Inject]
        private UserRegistrationConfirmationViewState UserRegistrationConfirmationViewState { get; set; } = null!;

        [Inject]
        private UserRegistrationConfirmationViewService UserRegistrationConfirmationViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationConfirmationViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationConfirmationViewState);
        }

        private async Task ConfirmAsync() => await UserRegistrationConfirmationViewService.ConfirmAsync();

        private async Task NavigateBack() => await UserRegistrationConfirmationViewService.NavigateBackAsync();

        private async Task RestartTimer() => await UserRegistrationConfirmationViewService.RestartTimerAsync();
    }
}
