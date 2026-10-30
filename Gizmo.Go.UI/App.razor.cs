using System.Globalization;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Common;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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

        [Inject]
        private IMobileOptionsService MobileOptionsService { get; set; } = null!;

        [Inject]
        private IUserLoginStatusService UserLoginStatusService { get; set; } = null!;

        [Inject]
        private ILogger<App> Logger { get; set; } = null!;

        private bool _initialized;
        private bool _loginStatusStarted;

        protected override async Task OnInitializedAsync()
        {
            // Association pattern required for Blazor Hybrid (MAUI/WPF) where
            // NavigationManager and IJSRuntime are not immediately available on host start.
            JSRuntimeService.AssociateJSRuntime(JSRuntime);
            NavigationService.AssociateNavigationManager(NavigationManager);

            try
            {
                await AuthService.TryRestoreSessionAsync();
                await SetServerCultureAsync();
                await ServiceProvider.InitializeViewsServices();
                await AppLifecycleService.InitializeAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "App initialization failed.");
            }
            finally
            {
                // the router is rendered even after a failed initialization, never an endless empty screen
                _initialized = true;
            }

            await base.OnInitializedAsync();
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            // pending agreements and profile completion of a restored session; started after the first render
            // of the router, because the agreement dialog needs the DialogHost of the layout
            if (_initialized && !_loginStatusStarted)
            {
                _loginStatusStarted = true;

                if (AuthService.State == AuthState.Authenticated)
                    _ = UserLoginStatusService.RunAsync(null);
            }

            return base.OnAfterRenderAsync(firstRender);
        }

        // MAUI only (Web always sets the culture in Program.cs before RunAsync): the start culture is the server one,
        // not persisted; must run before the view services are initialized (localized texts)
        private async Task SetServerCultureAsync()
        {
            if (CultureInfo.DefaultThreadCurrentUICulture is not null)
                return;

            // a failure keeps the neutral English culture; the screens show the connection error
            await CultureHelper.TrySetServerCultureAsync(MobileOptionsService, Logger);
        }
    }
}
