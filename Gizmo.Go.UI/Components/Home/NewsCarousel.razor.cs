using Gizmo.Go.Core.Models.News;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Go.UI.Components.Home
{
    /// <summary>
    /// Horizontal news carousel (CSS scroll-snap) ending with the "All news" card. Hidden without news.
    /// </summary>
    public partial class NewsCarousel : ComponentBase
    {
        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        private ElementReference _track;

        // the first news of the last render: a new first news is inserted left of the shown card and scroll-snap keeps the
        // old card, so the track is scrolled back to the start
        private int? _firstNewsId;

        [Parameter]
        public IReadOnlyList<NewsItem>? News { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public EventCallback<int> OnNewsClick { get; set; }

        [Parameter]
        public EventCallback OnAllNewsClick { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsLoading || News is not { Count: > 0 })
                return;

            var firstNewsId = News[0].Id;
            var isNewFirst = _firstNewsId.HasValue && _firstNewsId != firstNewsId;
            _firstNewsId = firstNewsId;

            if (!isNewFirst)
                return;

            try
            {
                await JSRuntime.InvokeVoidAsync("gizmoGoScroll.toStart", _track);
            }
            catch (JSException)
            {
                // the carousel works without the scroll
            }
        }
    }
}
