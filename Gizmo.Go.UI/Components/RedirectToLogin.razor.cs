using Gizmo.Go.UI.Helpers;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components
{
    public partial class RedirectToLogin : ComponentBase
    {
        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            // Build relative return URL from current location
            var currentUri = NavigationService.GetUri();
            var baseUri = NavigationService.GetBaseUri();
            var returnUrl = string.Empty;

            if (!string.IsNullOrEmpty(currentUri) && !string.IsNullOrEmpty(baseUri)
                && currentUri.StartsWith(baseUri, StringComparison.OrdinalIgnoreCase))
            {
                returnUrl = Uri.EscapeDataString(currentUri[baseUri.Length..]);
            }

            NavigationService.NavigateTo($"{NavigationHelper.LoginPage.TrimStart('/')}?returnUrl={returnUrl}");
        }
    }
}
