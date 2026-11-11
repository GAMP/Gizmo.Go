using Gizmo.Go.Core.Models.Agreements;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformUserAgreementsService : IUserAgreementsService
    {
        public Task<IReadOnlyList<UserAgreement>> GetAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
