using System.Reflection;
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
    /// "About the app" screen: the version and the app-side documents.
    /// </summary>
    [Register()]
    [Route(NavigationHelper.ProfileAboutRoute)]
    public sealed class ProfileAboutViewService : ViewStateServiceBase<ProfileAboutViewState>
    {
        private readonly NavigationService _navigationService;

        public ProfileAboutViewService(
            ProfileAboutViewState viewState,
            ILogger<ProfileAboutViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            ViewState.Version = GetVersion();
        }

        public void OpenDocument(int id) => _navigationService.NavigateTo(NavigationHelper.GetProfileAboutDocumentRoute(id), NavigationHelper.InAppNavigation);

        /// <summary>
        /// Returns to the previous screen; an entry opened directly (without an app history) opens the profile.
        /// </summary>
        /// <param name="historyEntryState">History entry state of the current screen.</param>
        public Task BackAsync(string? historyEntryState) => _navigationService.GoBackOrNavigateAsync(historyEntryState, NavigationHelper.ProfileRoute);

        // the informational version carries the build metadata after '+' (the commit), which is not a part of the version
        private static string GetVersion()
        {
            var assembly = typeof(App).Assembly;
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly.GetName().Version?.ToString()
                ?? string.Empty;

            var metadataIndex = version.IndexOf('+');
            return metadataIndex < 0 ? version : version[..metadataIndex];
        }
    }
}
