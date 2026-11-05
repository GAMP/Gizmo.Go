using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages
{
    public partial class NewsDetails : ComponentBase, IDisposable
    {
        private int _loadedId = -1;

        [Inject]
        private NewsDetailsViewState NewsDetailsViewState { get; set; } = null!;

        [Inject]
        private NewsDetailsViewService NewsDetailsViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter]
        public int Id { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(NewsDetailsViewState);
            base.OnInitialized();
        }

        // the view service has no route (a route with a parameter never matches its navigation events), the page starts the load
        protected override async Task OnParametersSetAsync()
        {
            if (_loadedId == Id)
                return;

            _loadedId = Id;
            await ReloadAsync();
            await base.OnParametersSetAsync();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(NewsDetailsViewState);
            NewsDetailsViewService.CancelLoad();
        }

        private Task ReloadAsync() => NewsDetailsViewService.LoadAsync(Id);

        private Task BackAsync() => NewsDetailsViewService.BackAsync(NavigationManager.HistoryEntryState);
    }
}
