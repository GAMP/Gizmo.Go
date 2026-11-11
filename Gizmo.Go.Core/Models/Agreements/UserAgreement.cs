namespace Gizmo.Go.Core.Models.Agreements
{
    /// <summary>
    /// Enabled club user agreement (policy) with its text.
    /// </summary>
    public sealed class UserAgreement
    {
        public int Id { get; init; }
        public string? Name { get; init; }

        /// <summary>
        /// Gets the agreement text (HTML).
        /// </summary>
        public string? Text { get; init; }

        public int DisplayOrder { get; init; }
    }
}
