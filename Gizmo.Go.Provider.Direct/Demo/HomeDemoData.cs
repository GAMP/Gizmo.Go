using Gizmo.Go.Core.Models;

namespace Gizmo.Go.Provider.Direct.Demo
{
    /// <summary>
    /// Demo data of the Home blocks that have no server endpoint yet.
    /// Edit this class to try other scenarios.
    /// The dated items are built on every read, so "today" stays today while the app runs.
    /// No images: the image places stay empty until the server provides the urls (the models and the UI already carry them).
    /// </summary>
    internal static class HomeDemoData
    {
        internal static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(300);

        internal static IReadOnlyList<Branch> Branches { get; } =
        [
            new Branch
            {
                Id = 1, Guid = new Guid("00000000-0000-0000-0000-000000000001"), Name = "Gizmo Arena", Address = "Rustaveli Ave, 12", City = "Tbilisi",
                HasBusinessSchedule = true, BusinessDayStart = new TimeOnly(0, 0), BusinessDayEnd = new TimeOnly(23, 59),
            },
            new Branch
            {
                Id = 2, Guid = new Guid("00000000-0000-0000-0000-000000000002"), Name = "Cyber Zone", Address = "Chavchavadze Ave, 33", City = " tbilisi ",
                HasBusinessSchedule = true, BusinessDayStart = new TimeOnly(10, 0), BusinessDayEnd = new TimeOnly(2, 0),
            },
            new Branch
            {
                Id = 3, Guid = new Guid("00000000-0000-0000-0000-000000000003"), Name = "Night Owl", Address = "Agmashenebeli Ave, 101", City = "Tbilisi",
                HasBusinessSchedule = true, BusinessDayStart = new TimeOnly(18, 0), BusinessDayEnd = new TimeOnly(6, 0),
                BusinessStartWeekDay = DayOfWeek.Friday, BusinessEndWeekDay = DayOfWeek.Sunday,
            },
            new Branch
            {
                Id = 4, Guid = new Guid("00000000-0000-0000-0000-000000000004"), Name = "Black Sea Gaming", Address = "Gorgiladze St, 7", City = "Batumi",
                HasBusinessSchedule = true, BusinessDayStart = new TimeOnly(12, 0), BusinessDayEnd = new TimeOnly(0, 0),
                BusinessStartWeekDay = DayOfWeek.Monday, BusinessEndWeekDay = DayOfWeek.Friday,
            },
            new Branch
            {
                Id = 5, Guid = new Guid("00000000-0000-0000-0000-000000000005"), Name = "Kutaisi Lounge", Address = "Tamar Mepe St, 20", City = "Kutaisi",
            },
        ];
    }
}
