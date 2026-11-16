using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Booking
{
    /// <summary>
    /// Payment method choice: the Gizmo balance (deposit) with its enough / not enough state; top up and card payment are inactive.
    /// </summary>
    public partial class BookingPaymentMethods : ComponentBase
    {
        [Inject]
        private BookingViewService Service { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter, EditorRequired]
        public BookingViewState State { get; set; } = null!;
    }
}
