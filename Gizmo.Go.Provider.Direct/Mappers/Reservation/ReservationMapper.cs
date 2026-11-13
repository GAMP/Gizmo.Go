using Gizmo.Go.Core.Models.Reservation;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Reservation
{
    internal static class ReservationMapper
    {
        internal static Core.Models.Reservation.Reservation Map(UserReservationModel source) =>
            new()
            {
                Id                      = source.Id,
                Date                    = source.Date,
                BranchDate              = source.BranchDate,
                Duration                = source.Duration,
                Status                  = MapStatus(source.Status),
                BranchId                = source.BranchId,
                BranchName              = source.BranchName,
                BranchAddress           = source.BranchAddress,
                IsOwner                 = source.IsOwner,
                Pin                     = source.Pin,
                PaymentStatus           = MapPaymentStatus(source.PaymentStatus),
                Total                   = source.Total,
                Outstanding             = source.Outstanding,
                CancellationGracePeriod = source.CancellationGracePeriod,
                Hosts                   = source.Hosts.Select(MapHost).ToList(),
            };

        private static ReservationHost MapHost(UserReservationHostModel source) =>
            new()
            {
                HostNumber    = source.HostNumber,
                HostName      = source.HostName,
                HostGroupName = source.HostGroupName,
                Status        = MapHostStatus(source.Status),
            };

        // unknown values fall back to the completed style, which has no actions
        private static GoReservationStatus MapStatus(ReservationStatus source) =>
            source switch
            {
                ReservationStatus.Waiting   => GoReservationStatus.Waiting,
                ReservationStatus.Canceled  => GoReservationStatus.Canceled,
                ReservationStatus.Active    => GoReservationStatus.Active,
                ReservationStatus.Expired   => GoReservationStatus.Expired,
                ReservationStatus.Completed => GoReservationStatus.Completed,
                ReservationStatus.Released  => GoReservationStatus.Released,
                _                           => GoReservationStatus.Completed,
            };

        private static GoReservationHostStatus MapHostStatus(ReservationHostStatus source) =>
            source switch
            {
                ReservationHostStatus.Waiting   => GoReservationHostStatus.Waiting,
                ReservationHostStatus.Canceled  => GoReservationHostStatus.Canceled,
                ReservationHostStatus.Active    => GoReservationHostStatus.Active,
                ReservationHostStatus.Expired   => GoReservationHostStatus.Expired,
                ReservationHostStatus.Completed => GoReservationHostStatus.Completed,
                ReservationHostStatus.Released  => GoReservationHostStatus.Released,
                _                               => GoReservationHostStatus.Completed,
            };

        private static GoReservationPaymentStatus MapPaymentStatus(ReservationPaymentStatus source) =>
            source switch
            {
                ReservationPaymentStatus.NotRequired        => GoReservationPaymentStatus.NotRequired,
                ReservationPaymentStatus.NotSatisfied       => GoReservationPaymentStatus.NotSatisfied,
                ReservationPaymentStatus.PartiallySatisfied => GoReservationPaymentStatus.PartiallySatisfied,
                ReservationPaymentStatus.Satisfied          => GoReservationPaymentStatus.Satisfied,
                _                                           => GoReservationPaymentStatus.NotRequired,
            };
    }
}
