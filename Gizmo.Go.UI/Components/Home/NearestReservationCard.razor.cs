using Gizmo.Go.Core.Models.Reservation;
using Gizmo.Go.UI.Helpers;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Home
{
    /// <summary>
    /// The nearest reservation of the user. The page shows the card only while it loads or when there is a reservation.
    /// </summary>
    public partial class NearestReservationCard : ComponentBase
    {
        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter]
        public Reservation? Reservation { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        /// <summary>
        /// Gets or sets the callback of "Open reservation".
        /// </summary>
        [Parameter]
        public EventCallback OnOpen { get; set; }

        private string? HostName => ReservationDisplayHelper.GetSingleHostName(Reservation!);

        private string DayText => ReservationDisplayHelper.GetDayText(Reservation!, LocalizationService);

        private string TimeText => ReservationDisplayHelper.GetTimeText(Reservation!);

        private bool ShowClubTimeHint => ReservationDisplayHelper.IsClubTimeHintNeeded(Reservation!);

        // an active reservation keeps the green pill, a waiting one takes the color of its payment
        private string StatusModifier => Reservation!.Status == GoReservationStatus.Active ? "" : Reservation.PaymentStatus switch
        {
            GoReservationPaymentStatus.PartiallySatisfied => "reservation-card__status--partial",
            GoReservationPaymentStatus.NotSatisfied => "reservation-card__status--unpaid",
            _ => ""
        };

        private string StatusText => LocalizationService.GetString(ReservationDisplayHelper.GetHomeStatusKey(Reservation!));
    }
}
