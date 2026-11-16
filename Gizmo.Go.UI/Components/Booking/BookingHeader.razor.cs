using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Booking
{
    /// <summary>
    /// Header of the booking flow: back and close buttons, title with the club name, the progress bar and the session chips.
    /// </summary>
    public partial class BookingHeader : ComponentBase
    {
        [Inject]
        private BookingViewService Service { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter, EditorRequired]
        public BookingViewState State { get; set; } = null!;

        private Task BackAsync() => Service.BackAsync(NavigationManager.HistoryEntryState);
    }
}
