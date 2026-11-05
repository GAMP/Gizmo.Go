using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages
{
    /// <summary>
    /// News details. The route has a parameter (<see cref="NavigationHelper.NewsDetailsRoute"/>), which the navigation events of
    /// the view services do not match, so the page starts the load with the id from the route.
    /// </summary>
    [Register()]
    public sealed class NewsDetailsViewService : ViewStateServiceBase<NewsDetailsViewState>
    {
        private readonly INewsService _newsService;
        private readonly NavigationService _navigationService;

        private CancellationTokenSource? _loadCts;

        public NewsDetailsViewService(
            NewsDetailsViewState viewState,
            ILogger<NewsDetailsViewService> logger,
            IServiceProvider serviceProvider,
            INewsService newsService,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _newsService = newsService;
            _navigationService = navigationService;
        }

        public async Task LoadAsync(int id)
        {
            CancelLoad();

            var cts = _loadCts = new CancellationTokenSource();
            ViewState.News.Reset();
            ViewState.IsNotFound = false;
            RaiseViewStateChanged();

            await ViewState.News.LoadAsync(ct => _newsService.GetAsync(id, ct), Logger, "news details", RaiseViewStateChanged, cts.Token);

            if (cts.IsCancellationRequested)
                return;

            ViewState.IsNotFound = !ViewState.News.HasError && ViewState.News.Value is null;
            RaiseViewStateChanged();
        }

        /// <summary>
        /// Cancels the running load (the page is left).
        /// </summary>
        public void CancelLoad()
        {
            _loadCts?.Cancel();
            _loadCts = null;
        }

        /// <summary>
        /// Returns to the previous screen; an entry opened directly (without an app history) opens the news list.
        /// </summary>
        /// <param name="historyEntryState">History entry state of the current screen.</param>
        public Task BackAsync(string? historyEntryState) => _navigationService.GoBackOrNavigateAsync(historyEntryState, NavigationHelper.NewsRoute);
    }
}
