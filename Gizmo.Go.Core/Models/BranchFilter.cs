namespace Gizmo.Go.Core.Models
{
    /// <summary>
    /// Branch query filter.
    /// </summary>
    public sealed class BranchFilter
    {
        /// <summary>
        /// Gets or sets search query (name, city).
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Gets or sets latitude for nearby search.
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Gets or sets longitude for nearby search.
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Gets or sets search radius in kilometers.
        /// </summary>
        public double? RadiusKm { get; set; }

        /// <summary>
        /// Gets or sets whether to filter by currently open branches.
        /// </summary>
        public bool? IsOpen { get; set; }
    }
}
