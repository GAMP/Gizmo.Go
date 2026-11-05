using Gizmo.Go.Core.Models.Ladder;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformLadderService : ILadderService
    {
        public Task<LadderLevel?> GetLevelAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
