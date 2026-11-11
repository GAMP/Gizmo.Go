using Gizmo.Go.UI.Components.Profile;
using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages
{
    public partial class ProfileAbout : ComponentBase, IDisposable
    {
        [Inject]
        private ProfileAboutViewState ProfileAboutViewState { get; set; } = null!;

        [Inject]
        private ProfileAboutViewService ProfileAboutViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(ProfileAboutViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(ProfileAboutViewState);
        }

        private IReadOnlyList<DocumentList.DocumentListItem> GetItems() =>
            Helpers.AboutDocuments.All
                .Select(x => new DocumentList.DocumentListItem(x.Id, LocalizationService.GetString(x.TitleKey)))
                .ToList();

        private void OpenDocument(int id) => ProfileAboutViewService.OpenDocument(id);

        private Task BackAsync() => ProfileAboutViewService.BackAsync(NavigationManager.HistoryEntryState);
    }
}
