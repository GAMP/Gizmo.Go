using Gizmo.Go.Core.Models.Ladder;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Achievement ladder of the current user.
    /// </summary>
    public interface ILadderService
    {
        /// <summary>
        /// Gets the current ladder level; null when the user has no level.
        /// </summary>
        Task<LadderLevel?> GetLevelAsync(CancellationToken cancellationToken = default);
    }
}
