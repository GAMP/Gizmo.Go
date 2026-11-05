using Gizmo.Go.Core.Models.Reservation;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Demo;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Reservations of the current user. Demo data until the reservations endpoint exists.
    /// </summary>
    internal sealed class DirectReservationService : IReservationService
    {
        // demo data until GET api/user/v3/reservations/nearest exists
        public async Task<NearestReservation?> GetNearestAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(HomeDemoData.Delay, cancellationToken);
            return HomeDemoData.NearestReservation;
        }
    }
}
