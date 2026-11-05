using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages
{
    public partial class NewsList : ComponentBase, IDisposable
    {
        [Inject]
        private NewsListViewState NewsListViewState { get; set; } = null!;

        [Inject]
        private NewsListViewService NewsListViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(NewsListViewState);
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await NewsListViewService.EnsureLoadedAsync();
            await base.OnInitializedAsync();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(NewsListViewState);
        }

        private void OnNewsClick(int id) => NewsListViewService.OpenNews(id);

        private Task BackAsync() => NewsListViewService.BackAsync(NavigationManager.HistoryEntryState);
    }
}
