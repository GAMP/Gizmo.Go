using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages
{
    public partial class Home : ComponentBase, IDisposable
    {
        [Inject]
        private HomeViewState HomeViewState { get; set; } = null!;

        [Inject]
        private HomeViewService HomeViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(HomeViewState);
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await HomeViewService.EnsureLoadedAsync();
            await base.OnInitializedAsync();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(HomeViewState);
        }

        private void OnNewsClick(int id) => HomeViewService.OpenNews(id);
    }
}
