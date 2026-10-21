using Gizmo.Go.UI.View.Services.Pages.PasswordRecovery;
using Gizmo.Go.UI.View.States.Pages.PasswordRecovery;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.PasswordRecovery
{
    public partial class PasswordRecoveryAccount : ComponentBase, IDisposable
    {
        [Inject] private PasswordRecoveryAccountViewState PasswordRecoveryAccountViewState { get; set; } = null!;

        [Inject] private PasswordRecoveryAccountViewService PasswordRecoveryAccountViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(PasswordRecoveryAccountViewState);
            base.OnInitialized();
        }

        private async Task ContinueAsync() => await PasswordRecoveryAccountViewService.ContinueAsync();

        private async Task ChooseAnotherMethodAsync() => await PasswordRecoveryAccountViewService.ChooseAnotherMethodAsync();

        private async Task NavigateBack() => await PasswordRecoveryAccountViewService.NavigateBackAsync();

        private async Task NavigateToLoginAsync() => await PasswordRecoveryAccountViewService.NavigateToLoginAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(PasswordRecoveryAccountViewState);
        }
    }
}
