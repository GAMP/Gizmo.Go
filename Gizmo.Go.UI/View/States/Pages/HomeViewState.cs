using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Models.Balance;
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

        public ViewBlock<NearestReservation> Reservation { get; } = new();

        /// <summary>
        /// Gets the latest news shown in the carousel.
        /// </summary>
        public ViewBlock<IReadOnlyList<NewsItem>> News { get; } = new();

        public ViewBlock<IReadOnlyList<string>> Cities { get; } = new();

        public string? SelectedCity { get; internal set; }

        /// <summary>
        /// Gets the clubs of the selected city.
        /// </summary>
        public ViewBlock<IReadOnlyList<Branch>> Clubs { get; } = new();
    }
}
