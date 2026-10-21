using Gizmo.Go.UI.View.Services.Pages.PasswordRecovery;
using Gizmo.Go.UI.View.States.Pages.PasswordRecovery;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.PasswordRecovery
{
    public partial class PasswordRecoverySuccess : ComponentBase, IDisposable
    {
        [Inject] private PasswordRecoverySuccessViewState PasswordRecoverySuccessViewState { get; set; } = null!;

        [Inject] private PasswordRecoverySuccessViewService PasswordRecoverySuccessViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(PasswordRecoverySuccessViewState);
            base.OnInitialized();
        }

        private async Task NavigateToLoginAsync() =>
            await PasswordRecoverySuccessViewService.NavigateToLoginAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(PasswordRecoverySuccessViewState);
        }
    }
}
