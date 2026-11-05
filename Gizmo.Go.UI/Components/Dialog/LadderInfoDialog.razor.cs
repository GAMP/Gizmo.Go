using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Dialog
{
    /// <summary>
    /// "How Ladder works": static explanation opened by the ladder level chip.
    /// </summary>
    public partial class LadderInfoDialog : ComponentBase
    {
        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

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

        private Task CloseAsync() => ResultCallback.InvokeAsync(new EmptyComponentResult());
    }
}
