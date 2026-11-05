namespace Gizmo.Go.Core.Models.Ladder
{
    /// <summary>
    /// Current achievement ladder level of the user.
    /// </summary>
    public sealed class LadderLevel
    {
        /// <summary>
        /// Gets the 1-based position of the level on the ladder.
        /// </summary>
        public int Ordinal { get; init; }

        /// <summary>
        /// Gets the level name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets the absolute URL of the level emblem; null when the level has none.
        /// </summary>
        public string? EmblemUrl { get; init; }
    }
}
