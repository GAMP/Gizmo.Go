using Gizmo.Go.Core.Models.News;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class NewsListViewState : ViewStateBase
    {
        public ViewBlock<IReadOnlyList<NewsItem>> News { get; } = new();
    }
}
