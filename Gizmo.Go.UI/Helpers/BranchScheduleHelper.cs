using System.Globalization;
using Gizmo.Go.Core.Models.Branch;

namespace Gizmo.Go.UI.Helpers
{
    /// <summary>
    /// Open state of a branch from its business schedule, computed when shown.
    /// </summary>
    internal static class BranchScheduleHelper
    {
        // the manager "Full-Time Mode" stores the day as 00:00 - 23:59
        private static readonly TimeOnly FullTimeEnd = new(23, 59);

        /// <summary>
        /// Gets whether the branch works the whole day (00:00 - 23:59).
        /// </summary>
        internal static bool IsAroundTheClock(Branch branch) =>
            branch.BusinessDayStart == TimeOnly.MinValue && branch.BusinessDayEnd == FullTimeEnd;

        /// <summary>
        /// Gets the business hours as "HH:mm{separator}HH:mm"; null when the branch has no business hours.
        /// </summary>
        internal static string? FormatHours(Branch branch, string separator = " - ") =>
            branch.BusinessDayStart is { } start && branch.BusinessDayEnd is { } end
                ? $"{start.ToString("HH:mm", CultureInfo.InvariantCulture)}{separator}{end.ToString("HH:mm", CultureInfo.InvariantCulture)}"
                : null;

        /// <summary>
        /// Gets the business week day of <paramref name="utcNow"/> in the branch time zone (the device time zone when it is
        /// unknown): the early hours of a day that passes midnight (18:00 - 06:00) belong to the previous day.
        /// </summary>
        internal static DayOfWeek GetBusinessDay(Branch branch, DateTime utcNow)
        {
            var now = ToBranchTime(branch, utcNow);

            if (branch.BusinessDayStart is { } start && branch.BusinessDayEnd is { } end && start > end
                && TimeOnly.FromDateTime(now) < end)
                return (DayOfWeek)(((int)now.DayOfWeek + 6) % 7);

            return now.DayOfWeek;
        }

        /// <summary>
        /// Gets whether the branch works on <paramref name="day"/>: a week that passes Sunday (Friday - Monday) wraps;
        /// no week days means every day.
        /// </summary>
        internal static bool IsBusinessWeekDay(Branch branch, DayOfWeek day)
        {
            if (branch.BusinessStartWeekDay is not { } first || branch.BusinessEndWeekDay is not { } last)
                return true;

            return first <= last
                ? day >= first && day <= last
                : day >= first || day <= last;
        }

        /// <summary>
        /// Gets whether the branch is open at <paramref name="utcNow"/> in the branch time zone (the device time zone when
        /// it is unknown); null when the branch has no business hours.
        /// </summary>
        /// <remarks>
        /// A day that passes midnight (18:00 - 06:00) belongs to its start week day; a week that passes Sunday
        /// (Friday - Monday) wraps; no week days means every day.
        /// </remarks>
        internal static bool? IsOpen(Branch branch, DateTime utcNow)
        {
            if (branch.BusinessDayStart is not { } start || branch.BusinessDayEnd is not { } end)
                return null;

            var now = ToBranchTime(branch, utcNow);
            var time = TimeOnly.FromDateTime(now);
            var day = now.DayOfWeek;

            if (IsAroundTheClock(branch))
                return IsBusinessWeekDay(branch, day);

            if (start <= end)
                return time >= start && time < end && IsBusinessWeekDay(branch, day);

            // passes midnight: the early hours belong to the previous day
            if (time >= start)
                return IsBusinessWeekDay(branch, day);

            return time < end && IsBusinessWeekDay(branch, (DayOfWeek)(((int)day + 6) % 7));
        }

        /// <summary>
        /// Gets the time zone of the branch; the device time zone when the branch zone is unknown.
        /// </summary>
        internal static TimeZoneInfo GetTimeZone(Branch branch) =>
            !string.IsNullOrEmpty(branch.TimeZone) && TimeZoneInfo.TryFindSystemTimeZoneById(branch.TimeZone, out var timeZone)
                ? timeZone
                : TimeZoneInfo.Local;

        /// <summary>
        /// Gets the wall-clock time of the branch at <paramref name="utcNow"/>.
        /// </summary>
        internal static DateTime ToBranchTime(Branch branch, DateTime utcNow) =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcNow, DateTimeKind.Utc), GetTimeZone(branch));

        /// <summary>
        /// Gets the UTC time of a branch wall-clock time; false when the local time does not exist or is ambiguous
        /// (a daylight saving change).
        /// </summary>
        internal static bool TryToUtc(Branch branch, DateTime branchTime, out DateTime utc) =>
            TryToUtc(GetTimeZone(branch), branchTime, out utc);

        /// <summary>
        /// Gets the UTC time of a wall-clock time of the zone; false when the local time does not exist or is ambiguous
        /// (a daylight saving change).
        /// </summary>
        internal static bool TryToUtc(TimeZoneInfo zone, DateTime branchTime, out DateTime utc)
        {
            var local = DateTime.SpecifyKind(branchTime, DateTimeKind.Unspecified);

            if (zone.IsInvalidTime(local) || zone.IsAmbiguousTime(local))
            {
                utc = default;
                return false;
            }

            utc = TimeZoneInfo.ConvertTimeToUtc(local, zone);
            return true;
        }
    }
}
