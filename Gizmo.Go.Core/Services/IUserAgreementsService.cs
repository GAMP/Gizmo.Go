using Gizmo.Go.Core.Models.Agreements;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Club user agreements (policies) available without authorization.
    /// </summary>
    public interface IUserAgreementsService
    {
        /// <summary>
        /// Gets the enabled agreements ordered by display order; failures are thrown to the caller.
        /// </summary>
        Task<IReadOnlyList<UserAgreement>> GetAsync(CancellationToken cancellationToken = default);
    }
}
