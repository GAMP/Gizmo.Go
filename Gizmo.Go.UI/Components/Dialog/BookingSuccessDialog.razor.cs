using Gizmo.Go.UI.Services.Dialog;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Dialog
{
    /// <summary>
    /// "Booked!" dialog: the summary of the new reservation, "My reservations" and "Home".
    /// </summary>
    public partial class BookingSuccessDialog : ComponentBase
    {
        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter]
        public string Hosts { get; set; } = string.Empty;

        [Parameter]
        public string Period { get; set; } = string.Empty;

        [Parameter]
        public string Cost { get; set; } = string.Empty;

        [Parameter]
        public DialogDisplayOptions? DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<BookingSuccessResult> ResultCallback { get; set; }

        // ErrorCallback, SuspendTimeoutCallback and Identifier are always supplied by the dialog controller (IComponentController.Parameters)
        [Parameter]
        public EventCallback<Exception> ErrorCallback { get; set; }

        [Parameter]
        public EventCallback<bool> SuspendTimeoutCallback { get; set; }

        [Parameter]
        public int Identifier { get; set; }

        private Task OpenReservationAsync() => ResultCallback.InvokeAsync(new BookingSuccessResult { OpenReservation = true });

        private Task GoHomeAsync() => ResultCallback.InvokeAsync(new BookingSuccessResult { OpenReservation = false });
    }
}
