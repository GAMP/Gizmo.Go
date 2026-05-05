using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationBotVerifySuccess : ComponentBase, IDisposable
    {
        [Inject] private RegistrationBotVerifySuccessViewState RegistrationBotVerifySuccessViewState { get; set; } = null!;

        [Inject] private RegistrationBotVerifySuccessViewService RegistrationBotVerifySuccessViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationBotVerifySuccessViewState);
            base.OnInitialized();
        }

        private async Task NavigateBackAsync() => await RegistrationBotVerifySuccessViewService.NavigateBackAsync();

        private async Task ConfirmAsync() => await RegistrationBotVerifySuccessViewService.ConfirmAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationBotVerifySuccessViewState);
        }
    }
}