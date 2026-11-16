using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Booking
{
    /// <summary>
    /// Bottom bar of the PC selection: the number of selected hosts, "Remove all", the total for the session,
    /// the expandable list of the selected hosts and the "To confirmation" button.
    /// </summary>
    public partial class BookingSelectionSummary : ComponentBase
    {
        [Inject]
        private BookingViewService Service { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter, EditorRequired]
        public BookingViewState State { get; set; } = null!;
    }
}
