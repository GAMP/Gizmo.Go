using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.Web.Components
{
    public partial class RedirectToLogin : ComponentBase
    {
        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        protected override void OnInitialized()
        {
            var returnUrl = Uri.EscapeDataString(Navigation.ToBaseRelativePath(Navigation.Uri));
            Navigation.NavigateTo($"login?returnUrl={returnUrl}", forceLoad: false);
        }
    }
}
