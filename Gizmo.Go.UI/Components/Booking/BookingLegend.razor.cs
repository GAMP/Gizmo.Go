using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Booking
{
    /// <summary>
    /// Legend of the host map: tile states and the host groups of the tab with their hourly rate.
    /// </summary>
    public partial class BookingLegend : ComponentBase
    {
        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter, EditorRequired]
        public BookingViewState State { get; set; } = null!;
    }
}
