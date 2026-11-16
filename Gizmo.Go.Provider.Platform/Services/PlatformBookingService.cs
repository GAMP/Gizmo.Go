using Gizmo.Go.Core.Models.Booking;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformBookingService : IBookingService
    {
        public Task<BookingOptions> GetOptionsAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<BookingHalls?> GetHallsAsync(int branchId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<BookingHostAvailability>> GetAvailabilityAsync(int branchId, DateTime startUtc, int duration, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<BookingHostGroupPrice>> GetPricesAsync(int branchId, DateTime startUtc, int duration, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<BookingOfferResult> CreateOfferAsync(int branchId, DateTime startUtc, int duration, IReadOnlyList<int> hostIds, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<BookingResult> BookAsync(Guid offerId, decimal amount, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
