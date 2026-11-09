using Gizmo.Go.Core.Models.Reservation;
using Api = Gizmo.Web.Api.Models;
using CoreReservation = Gizmo.Go.Core.Models.Reservation.Reservation;

namespace Gizmo.Go.Provider.Direct.Mappers.Reservation
{
    internal static class ReservationMapper
    {
        internal static CoreReservation Map(Api.UserReservationModel source) =>
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

        private static ReservationHost MapHost(Api.UserReservationHostModel source) =>
            new()
            {
                HostNumber    = source.HostNumber,
                HostName      = source.HostName,
                HostGroupName = source.HostGroupName,
                Status        = MapHostStatus(source.Status),
            };

        // unknown values fall back to the completed style, which has no actions
        private static ReservationStatus MapStatus(Api.ReservationStatus source) =>
            source switch
            {
                Api.ReservationStatus.Waiting   => ReservationStatus.Waiting,
                Api.ReservationStatus.Canceled  => ReservationStatus.Canceled,
                Api.ReservationStatus.Active    => ReservationStatus.Active,
                Api.ReservationStatus.Expired   => ReservationStatus.Expired,
                Api.ReservationStatus.Completed => ReservationStatus.Completed,
                Api.ReservationStatus.Released  => ReservationStatus.Released,
                _                               => ReservationStatus.Completed,
            };

        private static ReservationHostStatus MapHostStatus(Api.ReservationHostStatus source) =>
            source switch
            {
                Api.ReservationHostStatus.Waiting   => ReservationHostStatus.Waiting,
                Api.ReservationHostStatus.Canceled  => ReservationHostStatus.Canceled,
                Api.ReservationHostStatus.Active    => ReservationHostStatus.Active,
                Api.ReservationHostStatus.Expired   => ReservationHostStatus.Expired,
                Api.ReservationHostStatus.Completed => ReservationHostStatus.Completed,
                Api.ReservationHostStatus.Released  => ReservationHostStatus.Released,
                _                                   => ReservationHostStatus.Completed,
            };

        private static ReservationPaymentStatus MapPaymentStatus(Api.ReservationPaymentStatus source) =>
            source switch
            {
                Api.ReservationPaymentStatus.NotRequired        => ReservationPaymentStatus.NotRequired,
                Api.ReservationPaymentStatus.NotSatisfied       => ReservationPaymentStatus.NotSatisfied,
                Api.ReservationPaymentStatus.PartiallySatisfied => ReservationPaymentStatus.PartiallySatisfied,
                Api.ReservationPaymentStatus.Satisfied          => ReservationPaymentStatus.Satisfied,
                _                                               => ReservationPaymentStatus.NotRequired,
            };
    }
}
