using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Go.UI.Components.Dialog
{
    public partial class DialogHost : ComponentBase, IDisposable
    {
        [Inject]
        private DialogHostViewState DialogHostViewState { get; set; } = null!;

        // injected so the service is constructed and subscribes to DialogChanged
        [Inject]
        private DialogHostViewStateService DialogHostViewStateService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(DialogHostViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(DialogHostViewState);
        }

        private void OnClickDialogHandler(MouseEventArgs args)
        {
            if (DialogHostViewState.Current is { } current && current.DisplayOptions.CloseOnClick)
                _ = current.DismissAsync();
        }
    }
}
