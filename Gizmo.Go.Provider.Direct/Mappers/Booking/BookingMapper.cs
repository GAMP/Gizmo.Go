using Gizmo.Go.Core.Models.Booking;
using Gizmo.Server;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Booking
{
    internal static class BookingMapper
    {
        internal static BookingOptions Map(UserReservationOptionsModel source) =>
            new()
            {
                MinimumTime              = source.MinimumTime,
                MaximumTime              = source.MaximumTime,
                MultiHostDisable         = source.MultiHostDisable,
                FeeType                  = MapFeeType(source.FeeType),
                Fee                      = source.Fee,
                MinimumPaymentPercentage = source.MinimumPaymentPercentage,
                CancellationGracePeriod  = source.CancellationGracePeriod,
            };

        internal static BookingHalls Map(UserReservationHallsModel source) =>
            new()
            {
                Halls      = source.Halls.Select(Map).ToList(),
                Hosts      = source.Hosts.Select(Map).ToList(),
                HostGroups = source.HostGroups.Select(Map).ToList(),
            };

        internal static BookingHostAvailability Map(UserReservationHostAvailabilityModel source) =>
            new()
            {
                HostId      = source.HostId,
                IsAvailable = source.IsAvailable,
            };

        internal static BookingHostGroupPrice Map(UserReservationHostGroupPriceModel source) =>
            new()
            {
                HostGroupId = source.HostGroupId,
                Price       = source.Price,
                HourlyRate  = source.HourlyRate,
            };

        internal static BookingOfferResult Map(UserReservationOfferResultModel source) =>
            new()
            {
                Code               = MapOfferResult(source.Result),
                Offer              = source.Offer is null ? null : Map(source.Offer),
                UnavailableHostIds = source.UnavailableHostIds.ToList(),
            };

        internal static BookingResult Map(UserReservationBookResultModel source) =>
            new()
            {
                Code               = MapBookResult(source.Result),
                ReservationId      = source.ReservationId,
                UnavailableHostIds = source.UnavailableHostIds.ToList(),
            };

        private static BookingHall Map(UserReservationHallModel source) =>
            new()
            {
                Id           = source.Id,
                Name         = source.Name,
                DisplayOrder = source.DisplayOrder,
                Hosts        = source.Hosts.Select(Map).ToList(),
            };

        private static BookingHallHost Map(UserReservationHallHostModel source) =>
            new()
            {
                HostId = source.HostId,
                Row    = source.Row,
                Column = source.Column,
            };

        private static BookingHost Map(UserReservationBranchHostModel source) =>
            new()
            {
                Id          = source.Id,
                Number      = source.Number,
                Name        = source.Name,
                HostGroupId = source.HostGroupId,
            };

        private static BookingHostGroup Map(UserReservationHostGroupModel source) =>
            new()
            {
                Id   = source.Id,
                Name = source.Name,
            };

        private static BookingOffer Map(UserReservationOfferModel source) =>
            new()
            {
                OfferId        = source.OfferId,
                Date           = source.Date,
                Duration       = source.Duration,
                Hosts          = source.Hosts.Select(Map).ToList(),
                Fee            = source.Fee,
                Discount       = source.Discount,
                Total          = source.Total,
                MinimumPayment = source.MinimumPayment,
            };

        private static BookingOfferHost Map(UserReservationOfferHostModel source) =>
            new()
            {
                HostId      = source.HostId,
                HostGroupId = source.HostGroupId,
                Price       = source.Price,
            };

        private static BookingFeeType MapFeeType(ReservationFeeType source) =>
            source switch
            {
                ReservationFeeType.Fixed      => BookingFeeType.Fixed,
                ReservationFeeType.Percentage => BookingFeeType.Percentage,
                _                             => BookingFeeType.None,
            };

        // an unknown value is treated as the most restrictive result
        private static BookingOfferResultCode MapOfferResult(UserReservationOfferResult source) =>
            source switch
            {
                UserReservationOfferResult.Success          => BookingOfferResultCode.Success,
                UserReservationOfferResult.HostsUnavailable => BookingOfferResultCode.HostsUnavailable,
                _                                           => BookingOfferResultCode.NoPrice,
            };

        private static BookingResultCode MapBookResult(UserReservationBookResult source) =>
            source switch
            {
                UserReservationBookResult.Success             => BookingResultCode.Success,
                UserReservationBookResult.HostsUnavailable    => BookingResultCode.HostsUnavailable,
                UserReservationBookResult.InsufficientDeposit => BookingResultCode.InsufficientDeposit,
                _                                             => BookingResultCode.OfferExpired,
            };
    }
}
