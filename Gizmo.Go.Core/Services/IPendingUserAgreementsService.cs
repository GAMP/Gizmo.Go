using Gizmo.Go.Core.Models.Agreements;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// User agreements pending for the current user.
    /// </summary>
    public interface IPendingUserAgreementsService
    {
        Task<IReadOnlyList<PendingUserAgreement>> GetPendingAsync(CancellationToken cancellationToken = default);

        Task AcceptAsync(int userAgreementId, CancellationToken cancellationToken = default);
    }
}
