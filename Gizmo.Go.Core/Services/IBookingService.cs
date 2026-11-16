using Gizmo.Go.Core.Models.Booking;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Booking of PCs in a club for the current user.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Gets the reservation options.
        /// </summary>
        Task<BookingOptions> GetOptionsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the halls of the club with the hosts and host groups; null when the club does not exist or is not active.
        /// </summary>
        Task<BookingHalls?> GetHallsAsync(int branchId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets which hosts of the club are free for the period.
        /// </summary>
        /// <param name="startUtc">Session start (UTC).</param>
        /// <param name="duration">Session duration in minutes.</param>
        Task<IReadOnlyList<BookingHostAvailability>> GetAvailabilityAsync(int branchId, DateTime startUtc, int duration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the price of every bookable host group for the period.
        /// </summary>
        /// <param name="startUtc">Session start (UTC).</param>
        /// <param name="duration">Session duration in minutes.</param>
        Task<IReadOnlyList<BookingHostGroupPrice>> GetPricesAsync(int branchId, DateTime startUtc, int duration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates an offer for the chosen hosts and period.
        /// </summary>
        /// <param name="startUtc">Session start (UTC).</param>
        /// <param name="duration">Session duration in minutes.</param>
        Task<BookingOfferResult> CreateOfferAsync(int branchId, DateTime startUtc, int duration, IReadOnlyList<int> hostIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Books the offer, paying the amount from the user deposit.
        /// </summary>
        /// <param name="offerId">Offer id.</param>
        /// <param name="amount">Amount to pay: from the minimum payment up to the total, or 0 when the club minimum is 0.</param>
        Task<BookingResult> BookAsync(Guid offerId, decimal amount, CancellationToken cancellationToken = default);
    }
}
