using Gizmo.Go.Core.Models.Reservation;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Mappers.Reservation;
using Gizmo.Server.Exceptions;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using ReservationsClient = Gizmo.Web.Api.User.Clients.ReservationsWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server reservations (api/user/v3/reservations, secured).
    /// </summary>
    internal sealed class DirectReservationService : IReservationService
    {
        private readonly IServiceProvider _serviceProvider;

        public DirectReservationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<IReadOnlyList<Reservation>> GetCurrentAsync(CancellationToken cancellationToken = default) =>
            GetAsync(UserReservationsMode.Current, -1, cancellationToken);

        public Task<IReadOnlyList<Reservation>> GetCompletedAsync(int limit, CancellationToken cancellationToken = default) =>
            GetAsync(UserReservationsMode.Completed, limit, cancellationToken);

        public async Task<ReservationCancelResult> CancelAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _serviceProvider.GetRequiredService<ReservationsClient>();
                await client.CancelAsync(id, cancellationToken);
                return ReservationCancelResult.Success;
            }
            catch (Exception ex) when (ex.IsExceptionCode(ExceptionCode.EntityNotFound) || ex.IsExceptionCode(ExceptionCode.HostReservation))
            {
                return ReservationCancelResult.NotAvailable;
            }
        }

        // the sorting is set by the server according to the mode
        private async Task<IReadOnlyList<Reservation>> GetAsync(UserReservationsMode mode, int limit, CancellationToken cancellationToken)
        {
            var client = _serviceProvider.GetRequiredService<ReservationsClient>();
            var filter = new UserReservationsFilter
            {
                Mode = mode,
                Pagination = new ModelFilterPagination { Limit = limit },
            };

            var page = await client.GetAsync(filter, cancellationToken);
            return page.Data.Select(ReservationMapper.Map).ToList();
        }
    }
}
