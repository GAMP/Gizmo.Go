using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages
{
    public partial class Profile : ComponentBase, IDisposable
    {
        [Inject]
        private ProfileViewState ProfileViewState { get; set; } = null!;

        [Inject]
        private ProfileViewService ProfileViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(ProfileViewState);
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await ProfileViewService.EnsureLoadedAsync();
            await base.OnInitializedAsync();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(ProfileViewState);
        }

        private Task BackAsync() => ProfileViewService.BackAsync(NavigationManager.HistoryEntryState);
    }
}
