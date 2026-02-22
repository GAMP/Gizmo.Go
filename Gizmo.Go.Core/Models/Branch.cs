namespace Gizmo.Go.Core.Models
{
    /// <summary>
    /// Branch (location) model for end-user display.
    /// </summary>
    public sealed class Branch
    {
        /// <summary>
        /// Gets or sets branch id.
        /// </summary>
        public required int Id { get; init; }

        /// <summary>
        /// Gets or sets branch unique identifier.
        /// </summary>
        public required Guid Guid { get; init; }

        /// <summary>
        /// Gets or sets branch name.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Gets or sets business name.
        /// </summary>
        public string? BusinessName { get; init; }

        /// <summary>
        /// Gets or sets address.
        /// </summary>
        public string? Address { get; init; }

        /// <summary>
        /// Gets or sets city.
        /// </summary>
        public string? City { get; init; }

        /// <summary>
        /// Gets or sets country.
        /// </summary>
        public string? Country { get; init; }

        /// <summary>
        /// Gets or sets region.
        /// </summary>
        public string? Region { get; init; }

        /// <summary>
        /// Gets or sets postal code.
        /// </summary>
        public string? PostalCode { get; init; }

        /// <summary>
        /// Gets or sets latitude.
        /// </summary>
        public decimal? Latitude { get; init; }

        /// <summary>
        /// Gets or sets longitude.
        /// </summary>
        public decimal? Longitude { get; init; }

        /// <summary>
        /// Gets or sets phone number.
        /// </summary>
        public string? Phone { get; init; }

        /// <summary>
        /// Gets or sets email.
        /// </summary>
        public string? Email { get; init; }

        /// <summary>
        /// Gets or sets website.
        /// </summary>
        public string? WebSite { get; init; }

        /// <summary>
        /// Gets or sets description/info.
        /// </summary>
        public string? Info { get; init; }

        /// <summary>
        /// Gets or sets timezone identifier.
        /// </summary>
        public string? TimeZone { get; init; }

        /// <summary>
        /// Gets or sets business day start time.
        /// </summary>
        public TimeOnly? BusinessDayStart { get; init; }

        /// <summary>
        /// Gets or sets business day end time.
        /// </summary>
        public TimeOnly? BusinessDayEnd { get; init; }
    }
}
