using Gizmo.Go.Core.Models;

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

        private static bool IsBusinessWeekDay(Branch branch, DayOfWeek day)
        {
            if (branch.BusinessStartWeekDay is not { } first || branch.BusinessEndWeekDay is not { } last)
                return true;

            return first <= last
                ? day >= first && day <= last
                : day >= first || day <= last;
        }

        private static DateTime ToBranchTime(Branch branch, DateTime utcNow)
        {
            if (!string.IsNullOrEmpty(branch.TimeZone) && TimeZoneInfo.TryFindSystemTimeZoneById(branch.TimeZone, out var timeZone))
                return TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);

            return utcNow.ToLocalTime();
        }
    }
}
