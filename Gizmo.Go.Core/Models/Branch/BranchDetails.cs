namespace Gizmo.Go.Core.Models.Branch
{
    /// <summary>
    /// Active branch (club) with its computer counts and host group names.
    /// </summary>
    public sealed class BranchDetails
    {
        /// <summary>
        /// Gets or sets the branch.
        /// </summary>
        public required Branch Branch { get; init; }

        /// <summary>
        /// Gets or sets the number of non-deleted computers in the branch.
        /// </summary>
        public int TotalComputers { get; init; }

        /// <summary>
        /// Gets or sets the number of computers free now.
        /// </summary>
        public int FreeComputers { get; init; }

        /// <summary>
        /// Gets or sets the names of the host groups that have computers, ordered by name.
        /// </summary>
        public IReadOnlyList<string> HostGroups { get; init; } = [];
    }
}
