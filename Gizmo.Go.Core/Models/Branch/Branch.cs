namespace Gizmo.Go.Core.Models.Branch
{
    /// <summary>
    /// Active branch (club) as the server returns it to the user. The business schedule is the effective one: the branch
    /// value, otherwise the server-wide one.
    /// </summary>
    public sealed class Branch
    {
        /// <summary>
        /// Gets or sets branch id.
        /// </summary>
        public required int Id { get; init; }

        /// <summary>
        /// Gets or sets branch name.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Gets or sets country.
        /// </summary>
        public string? Country { get; init; }

        /// <summary>
        /// Gets or sets city.
        /// </summary>
        public string? City { get; init; }

        /// <summary>
        /// Gets or sets address.
        /// </summary>
        public string? Address { get; init; }

        /// <summary>
        /// Gets or sets postal code.
        /// </summary>
        public string? PostalCode { get; init; }

        /// <summary>
        /// Gets or sets region.
        /// </summary>
        public string? Region { get; init; }

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
        /// Gets or sets the IANA time zone identifier of the branch.
        /// </summary>
        public required string TimeZone { get; init; }

        /// <summary>
        /// Gets or sets business start week day.
        /// </summary>
        public DayOfWeek? BusinessStartWeekDay { get; init; }

        /// <summary>
        /// Gets or sets business end week day.
        /// </summary>
        public DayOfWeek? BusinessEndWeekDay { get; init; }

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
