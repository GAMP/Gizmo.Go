using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages
{
    public partial class ProfileLanguage : ComponentBase, IDisposable
    {
        [Inject]
        private ProfileLanguageViewState ProfileLanguageViewState { get; set; } = null!;

        [Inject]
        private ProfileLanguageViewService ProfileLanguageViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(ProfileLanguageViewState);

            // the list is built here as well: the screen can be opened without a navigation event (a direct url)
            ProfileLanguageViewService.Load();
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(ProfileLanguageViewState);
        }

        private Task SelectAsync(string code) => ProfileLanguageViewService.SelectAsync(code, NavigationManager.HistoryEntryState);

        private Task BackAsync() => ProfileLanguageViewService.BackAsync(NavigationManager.HistoryEntryState);
    }
}
