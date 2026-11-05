using Gizmo.Go.Core.Models.Balance;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Balance of the current user.
    /// </summary>
    public interface IUserBalanceService
    {
        /// <summary>
        /// Gets the balance and the bonus points of the current user.
        /// </summary>
        Task<UserBalance> GetAsync(CancellationToken cancellationToken = default);
    }
}
