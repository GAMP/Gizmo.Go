using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Dialog
{
    /// <summary>
    /// Confirmation dialog: the confirm button completes the dialog with a result, "Cancel" dismisses it.
    /// </summary>
    public partial class ConfirmDialog : ComponentBase
    {
        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public string Text { get; set; } = string.Empty;

        [Parameter]
        public string ConfirmText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the confirm button is shown in the danger color.
        /// </summary>
        [Parameter]
        public bool IsDanger { get; set; }

        [Parameter]
        public DialogDisplayOptions? DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<EmptyComponentResult> ResultCallback { get; set; }

        // ErrorCallback, SuspendTimeoutCallback and Identifier are always supplied by the dialog controller (IComponentController.Parameters)
        [Parameter]
        public EventCallback<Exception> ErrorCallback { get; set; }

        [Parameter]
        public EventCallback<bool> SuspendTimeoutCallback { get; set; }

        [Parameter]
        public int Identifier { get; set; }

        private Task ConfirmAsync() => ResultCallback.InvokeAsync(new EmptyComponentResult());

        private Task CancelAsync() => DismissCallback.InvokeAsync();
    }
}
