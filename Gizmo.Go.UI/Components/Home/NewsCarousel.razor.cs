using Gizmo.Go.Core.Models.News;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Home
{
    /// <summary>
    /// Horizontal news carousel (CSS scroll-snap) ending with the "All news" card. Hidden without news.
    /// </summary>
    public partial class NewsCarousel : ComponentBase
    {
        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter]
        public IReadOnlyList<NewsItem>? News { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public EventCallback<int> OnNewsClick { get; set; }

        [Parameter]
        public EventCallback OnAllNewsClick { get; set; }
    }
}
