using Gizmo.Go.UI.View.Services.Pages.PasswordRecovery;
using Gizmo.Go.UI.View.States.Pages.PasswordRecovery;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.PasswordRecovery
{
    public partial class PasswordRecovery : ComponentBase, IDisposable
    {
        [Inject]
        private PasswordRecoveryViewState PasswordRecoveryViewState { get; set; } = null!;

        [Inject]
        private PasswordRecoveryViewService PasswordRecoveryViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(PasswordRecoveryViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(PasswordRecoveryViewState);
        }

        private async Task SelectProvider(int methodId) =>
            await PasswordRecoveryViewService.SelectProviderAsync(methodId);

        private async Task NavigateBack() => await PasswordRecoveryViewService.NavigateBackAsync();
    }
}
