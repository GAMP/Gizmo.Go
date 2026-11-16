using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Booking
{
    /// <summary>
    /// Step 2 of the booking: hall tabs, the host map or list, the legend and the summary of the selected hosts.
    /// </summary>
    public partial class BookingHostsStep : ComponentBase
    {
        [Inject]
        private BookingViewService Service { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter, EditorRequired]
        public BookingViewState State { get; set; } = null!;
    }
}
