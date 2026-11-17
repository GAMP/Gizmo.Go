using Gizmo.Go.UI.Services.Dialog;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Dialog
{
    /// <summary>
    /// "Host already taken" sheet of the booking: the hosts that are no longer free and the way back to the PC selection
    /// or to the session time.
    /// </summary>
    public partial class BookingHostTakenDialog : ComponentBase
    {
        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter]
        public IReadOnlyList<BookingHostTakenItem> Hosts { get; set; } = [];

        [Parameter]
        public DialogDisplayOptions? DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<BookingHostTakenResult> ResultCallback { get; set; }

        // ErrorCallback, SuspendTimeoutCallback and Identifier are always supplied by the dialog controller (IComponentController.Parameters)
        [Parameter]
        public EventCallback<Exception> ErrorCallback { get; set; }

        [Parameter]
        public EventCallback<bool> SuspendTimeoutCallback { get; set; }

        [Parameter]
        public int Identifier { get; set; }

        private Task ReturnAsync() => ResultCallback.InvokeAsync(new BookingHostTakenResult { ChangeTime = false });

        private Task ChangeTimeAsync() => ResultCallback.InvokeAsync(new BookingHostTakenResult { ChangeTime = true });
    }
}
