using Gizmo.Go.Core.Models.Booking;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Mappers.Booking;
using Gizmo.Server.Exceptions;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using ReservationsClient = Gizmo.Web.Api.User.Clients.ReservationsWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server booking (api/user/v3/reservations: options, halls, availability, prices, offer, book; secured).
    /// </summary>
    internal sealed class DirectBookingService : IBookingService
    {
        private readonly IServiceProvider _serviceProvider;

        public DirectBookingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<BookingOptions> GetOptionsAsync(CancellationToken cancellationToken = default)
        {
            var model = await Client.OptionsAsync(cancellationToken);
            return BookingMapper.Map(model);
        }

        public async Task<BookingHalls?> GetHallsAsync(int branchId, CancellationToken cancellationToken = default)
        {
            try
            {
                var model = await Client.HallsAsync(branchId, cancellationToken);
                return BookingMapper.Map(model);
            }
            catch (Exception ex) when (ex.IsExceptionCode(ExceptionCode.EntityNotFound))
            {
                return null;
            }
        }

        public async Task<IReadOnlyList<BookingHostAvailability>> GetAvailabilityAsync(int branchId, DateTime startUtc, int duration, CancellationToken cancellationToken = default)
        {
            var models = await Client.AvailabilityAsync(CreateFilter(branchId, startUtc, duration), cancellationToken);
            return models.Select(BookingMapper.Map).ToList();
        }

        public async Task<IReadOnlyList<BookingHostGroupPrice>> GetPricesAsync(int branchId, DateTime startUtc, int duration, CancellationToken cancellationToken = default)
        {
            var models = await Client.PricesAsync(CreateFilter(branchId, startUtc, duration), cancellationToken);
            return models.Select(BookingMapper.Map).ToList();
        }

        public async Task<BookingOfferResult> CreateOfferAsync(int branchId, DateTime startUtc, int duration, IReadOnlyList<int> hostIds, CancellationToken cancellationToken = default)
        {
            var model = new UserReservationOfferCreateModel
            {
                BranchId = branchId,
                Date     = startUtc,
                Duration = duration,
                Hosts    = hostIds.ToList(),
            };

            var result = await Client.OfferAsync(model, cancellationToken);
            return BookingMapper.Map(result);
        }

        public async Task<BookingResult> BookAsync(Guid offerId, decimal amount, CancellationToken cancellationToken = default)
        {
            var model = new UserReservationBookModel
            {
                OfferId = offerId,
                Amount  = amount,
            };

            var result = await Client.BookAsync(model, cancellationToken);
            return BookingMapper.Map(result);
        }

        private ReservationsClient Client => _serviceProvider.GetRequiredService<ReservationsClient>();

        private static UserReservationPeriodFilter CreateFilter(int branchId, DateTime startUtc, int duration) =>
            new()
            {
                BranchId = branchId,
                Start    = startUtc,
                Duration = duration,
            };
    }
}
