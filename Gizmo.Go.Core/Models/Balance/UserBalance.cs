namespace Gizmo.Go.Core.Models.Balance
{
    /// <summary>
    /// Balance of the current user.
    /// </summary>
    public sealed class UserBalance
    {
        /// <summary>
        /// Gets the main balance.
        /// </summary>
        public decimal Balance { get; init; }

        /// <summary>
        /// Gets the bonus points.
        /// </summary>
        public int Points { get; init; }
    }
}
