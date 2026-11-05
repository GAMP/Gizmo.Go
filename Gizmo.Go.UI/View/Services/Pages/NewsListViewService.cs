using Gizmo.Go.Core.Models.News;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages
{
    /// <summary>
    /// News list ("All news"). No paging in the first version.
    /// </summary>
    [Register()]
    [Route(NavigationHelper.NewsRoute)]
    public sealed class NewsListViewService : ViewStateServiceBase<NewsListViewState>
    {
        private readonly IAuthService _authService;
        private readonly INewsService _newsService;
        private readonly NavigationService _navigationService;

        private CancellationTokenSource? _loadCts;

        public NewsListViewService(
            NewsListViewState viewState,
            ILogger<NewsListViewService> logger,
            IServiceProvider serviceProvider,
            IAuthService authService,
            INewsService newsService,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _authService = authService;
            _newsService = newsService;
            _navigationService = navigationService;
        }

        /// <summary>
        /// Loads the news when the screen was opened without a navigation event (a direct url).
        /// </summary>
        public Task EnsureLoadedAsync() => _loadCts is null ? LoadAsync() : Task.CompletedTask;

        public Task LoadAsync()
        {
            CancelLoad();

            _loadCts = new CancellationTokenSource();
            ViewState.News.Reset();
            RaiseViewStateChanged();

            return ViewState.News.LoadAsync(
                async ct => (IReadOnlyList<NewsItem>)await _newsService.GetListAsync(null, ct),
                Logger, "news list", RaiseViewStateChanged, _loadCts.Token);
        }

        public void OpenNews(int id) => _navigationService.NavigateTo(NavigationHelper.GetNewsDetailsRoute(id), NavigationHelper.InAppNavigation);

        /// <summary>
        /// Returns to the previous screen; an entry opened directly (without an app history) opens Home.
        /// </summary>
        /// <param name="historyEntryState">History entry state of the current screen.</param>
        public Task BackAsync(string? historyEntryState) => _navigationService.GoBackOrNavigateAsync(historyEntryState, NavigationHelper.HomePage);

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            // an unauthenticated user is redirected to Welcome by the page, there is nothing to load
            if (_authService.State != AuthState.Authenticated)
                return Task.CompletedTask;

            return LoadAsync();
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            CancelLoad();
            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        private void CancelLoad()
        {
            _loadCts?.Cancel();
            _loadCts = null;
        }
    }
}
