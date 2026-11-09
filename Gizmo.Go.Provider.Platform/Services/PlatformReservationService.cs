using Gizmo.Go.Core.Models.Reservation;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformReservationService : IReservationService
    {
        public Task<IReadOnlyList<Reservation>> GetCurrentAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<Reservation>> GetCompletedAsync(int limit, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<ReservationCancelResult> CancelAsync(int id, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
