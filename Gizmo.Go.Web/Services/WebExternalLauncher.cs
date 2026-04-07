using Gizmo.Go.Core.Services;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.Web.Services
{
    /// <summary>
    /// Opens an external URL via <see cref="NavigationManager.NavigateTo"/> with ForceLoad,
    /// which hands the URL to the browser and lets the OS handle it (browser tab or app deep link).
    /// </summary>
    public sealed class WebExternalLauncher : IExternalLauncher
    {
        private readonly NavigationService _navigationService;

        public WebExternalLauncher(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public Task OpenAsync(string url, CancellationToken cancellationToken = default)
        {
            _navigationService.NavigateTo(url, new NavigationOptions { ForceLoad = true });
            return Task.CompletedTask;
        }
    }
}
