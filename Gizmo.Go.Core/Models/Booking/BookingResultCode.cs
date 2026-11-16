namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// Result code of booking an offer.
    /// </summary>
    public enum BookingResultCode
    {
        /// <summary>
        /// The reservation is created.
        /// </summary>
        Success,

        /// <summary>
        /// Some of the hosts are no longer free.
        /// </summary>
        HostsUnavailable,

        /// <summary>
        /// The deposit is lower than the amount.
        /// </summary>
        InsufficientDeposit,

        /// <summary>
        /// The offer no longer exists (expired or not found).
        /// </summary>
        OfferExpired
    }
}
