using Gizmo.Go.UI.Helpers;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components
{
    public partial class RedirectToWelcome : ComponentBase
    {
        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        // an unauthenticated user always starts from Welcome; after the login the app opens home
        protected override void OnInitialized() =>
            NavigationService.NavigateTo(NavigationHelper.WelcomePage);
    }
}
