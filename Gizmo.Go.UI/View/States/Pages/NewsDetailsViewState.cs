using Gizmo.Go.Core.Models.News;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class NewsDetailsViewState : ViewStateBase
    {
        public ViewBlock<NewsItem> News { get; } = new();

        /// <summary>
        /// Gets whether the news does not exist (the load succeeded without a result).
        /// </summary>
        public bool IsNotFound { get; internal set; }
    }
}
