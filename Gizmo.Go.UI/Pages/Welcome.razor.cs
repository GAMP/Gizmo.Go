using Gizmo.Go.UI.View.Services;
using Gizmo.Go.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages
{
    public partial class Welcome : ComponentBase, IDisposable
    {
        [Inject]
        private WelcomeViewService WelcomeViewService { get; set; } = null!;

        [Inject]
        private RegistrationConfigurationViewState RegistrationConfigurationViewState { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationConfigurationViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationConfigurationViewState);
        }

        private async Task NavigateToLogin() => await WelcomeViewService.NavigateToLoginAsync();

        private async Task NavigateToRegister() => await WelcomeViewService.NavigateToRegisterAsync();

        private async Task ContinueAsGuest() => await WelcomeViewService.ContinueAsGuestAsync();
    }
}
