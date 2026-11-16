using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Dialog
{
    /// <summary>
    /// "No options" dialog of the booking: no free PC for the chosen session. "Choose another time" completes the dialog,
    /// "Close" dismisses it.
    /// </summary>
    public partial class BookingNoOptionsDialog : ComponentBase
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

        private Task ChooseTimeAsync() => ResultCallback.InvokeAsync(new EmptyComponentResult());

        private Task CloseAsync() => DismissCallback.InvokeAsync();
    }
}
