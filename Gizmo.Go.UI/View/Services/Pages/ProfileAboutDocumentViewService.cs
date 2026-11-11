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
    /// Text of an app-side document. The route has a parameter (<see cref="NavigationHelper.ProfileAboutDocumentRoute"/>), which
    /// the navigation events of the view services do not match, so the page passes the id from the route.
    /// </summary>
    [Register()]
    public sealed class ProfileAboutDocumentViewService : ViewStateServiceBase<ProfileAboutDocumentViewState>
    {
        private readonly ILocalizationService _localizationService;
        private readonly NavigationService _navigationService;

        public ProfileAboutDocumentViewService(
            ProfileAboutDocumentViewState viewState,
            ILogger<ProfileAboutDocumentViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _navigationService = navigationService;
        }

        public void Load(int id)
        {
            var document = AboutDocuments.Find(id);

            ViewState.IsNotFound = document is null;
            ViewState.Title = document is null ? string.Empty : _localizationService.GetString(document.TitleKey);
            ViewState.Text = document is null ? string.Empty : _localizationService.GetString(document.TextKey);
            RaiseViewStateChanged();
        }

        /// <summary>
        /// Returns to the previous screen; an entry opened directly (without an app history) opens the "About the app" screen.
        /// </summary>
        /// <param name="historyEntryState">History entry state of the current screen.</param>
        public Task BackAsync(string? historyEntryState) => _navigationService.GoBackOrNavigateAsync(historyEntryState, NavigationHelper.ProfileAboutRoute);
    }
}
