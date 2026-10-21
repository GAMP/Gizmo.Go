using Gizmo.Go.Core.Models.PasswordRecovery;
using Gizmo.Go.UI.View.Services.Pages.PasswordRecovery;
using Gizmo.Go.UI.View.States.Pages.PasswordRecovery;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.PasswordRecovery
{
    public partial class PasswordRecoveryKind : ComponentBase, IDisposable
    {
        private static readonly Guid EmailChannelGuid = new(CommunicationChannels.Email);

        [Inject]
        private PasswordRecoveryKindViewState PasswordRecoveryKindViewState { get; set; } = null!;

        [Inject]
        private PasswordRecoveryKindViewService PasswordRecoveryKindViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(PasswordRecoveryKindViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(PasswordRecoveryKindViewState);
        }

        private void SelectKind(PasswordRecoveryIdentifierKind kind) =>
            PasswordRecoveryKindViewService.SelectKind(kind);

        private async Task NavigateBack() => await PasswordRecoveryKindViewService.NavigateBackAsync();
    }
}
