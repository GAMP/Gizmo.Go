using Gizmo.Go.Core.Models.Reservation;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformReservationService : IReservationService
    {
        public Task<NearestReservation?> GetNearestAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
