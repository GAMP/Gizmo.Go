using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Booking
{
    /// <summary>
    /// Tabs of the halls with the number of free hosts of each.
    /// </summary>
    public partial class BookingHallTabs : ComponentBase
    {
        [Inject]
        private BookingViewService Service { get; set; } = null!;

        [Parameter, EditorRequired]
        public BookingViewState State { get; set; } = null!;
    }
}
