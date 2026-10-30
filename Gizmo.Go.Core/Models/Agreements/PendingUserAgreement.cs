namespace Gizmo.Go.Core.Models.Agreements
{
    /// <summary>
    /// User agreement the current user has to accept.
    /// </summary>
    public sealed class PendingUserAgreement
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Agreement { get; init; }
        public bool IsRejectable { get; init; }

        /// <summary>
        /// Gets whether the agreement is shown on every app start and its acceptance is not stored.
        /// </summary>
        public bool IgnoreState { get; init; }
    }
}
