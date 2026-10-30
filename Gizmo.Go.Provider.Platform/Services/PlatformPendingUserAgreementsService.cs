using Gizmo.Go.Core.Models.Agreements;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformPendingUserAgreementsService : IPendingUserAgreementsService
    {
        public Task<IReadOnlyList<PendingUserAgreement>> GetPendingAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task AcceptAsync(int userAgreementId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
