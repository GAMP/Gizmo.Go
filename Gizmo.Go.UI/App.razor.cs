using Gizmo.Go.Core.Services;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Go.UI
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

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IServiceProvider ServiceProvider { get; set; } = null!;

        [Inject]
        private IAppLifecycleService AppLifecycleService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            // Association pattern required for Blazor Hybrid (MAUI/WPF) where
            // NavigationManager and IJSRuntime are not immediately available on host start.
            JSRuntimeService.AssociateJSRuntime(JSRuntime);
            NavigationService.AssociateNavigationManager(NavigationManager);

            await AuthService.TryRestoreSessionAsync();
            await ServiceProvider.InitializeViewsServices();
            await AppLifecycleService.InitializeAsync();

            await base.OnInitializedAsync();
        }
    }
}
