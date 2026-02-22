using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Go.Web
{
    public partial class App : ComponentBase
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Inject]
        private JSRuntimeService JSRuntimeService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            JSRuntimeService.AssociateJSRuntime(JSRuntime);
            NavigationService.AssociateNavigationManager(NavigationManager);

            await base.OnInitializedAsync();
        }
    }
}
