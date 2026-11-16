using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Booking
{
    /// <summary>
    /// List of the hosts of a tab, a section per host group.
    /// </summary>
    public partial class BookingHostList : ComponentBase
    {
        [Inject]
        private BookingViewService Service { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter, EditorRequired]
        public BookingViewState State { get; set; } = null!;
    }
}
