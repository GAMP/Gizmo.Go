using Gizmo.Go.Core.Models.Balance;
using Gizmo.Go.Core.Models.Branch;
using Gizmo.Go.Core.Models.Ladder;
using Gizmo.Go.Core.Models.News;
using Gizmo.Go.Core.Models.Reservation;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class HomeViewState : ViewStateBase
    {
        public ViewBlock<string> Logo { get; } = new();

        public ViewBlock<LadderLevel> Level { get; } = new();

        public ViewBlock<UserBalance> Balance { get; } = new();

        public ViewBlock<Reservation> Reservation { get; } = new();

        /// <summary>
        /// Gets the latest news shown in the carousel.
        /// </summary>
        public ViewBlock<IReadOnlyList<NewsItem>> News { get; } = new();

        /// <summary>
        /// Gets all active clubs.
        /// </summary>
        public ViewBlock<IReadOnlyList<Branch>> Clubs { get; } = new();

        /// <summary>
        /// Gets the selected city; null means all cities.
        /// </summary>
        public string? SelectedCity { get; internal set; }

        /// <summary>
        /// Gets the distinct cities of the loaded clubs (trimmed, case-insensitive, by name); clubs without a city add none.
        /// </summary>
        public IReadOnlyList<string> Cities => (Clubs.Value ?? [])
            .Select(club => club.City?.Trim())
            .Where(city => !string.IsNullOrEmpty(city))
            .Cast<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();

        /// <summary>
        /// Gets the clubs of the selected city; all clubs (with or without a city) when no city is selected.
        /// </summary>
        public IReadOnlyList<Branch> ShownClubs => SelectedCity is not { } city
            ? Clubs.Value ?? []
            : (Clubs.Value ?? []).Where(club => string.Equals(club.City?.Trim(), city, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
