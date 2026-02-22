using System.Globalization;
using Gizmo;
using Gizmo.Go.Core.Configuration;
using Gizmo.Go.Core.Extensions;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Maui.Services;
using Gizmo.Go.Provider.Direct.Extensions;
using Gizmo.Go.Provider.Platform.Extensions;
using Gizmo.Go.UI.Providers;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // configuration from embedded appsettings.json
            var stream = typeof(MauiProgram).Assembly.GetManifestResourceStream("Gizmo.Go.Maui.wwwroot.appsettings.json");
            if (stream is not null)
            {
                var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
                builder.Configuration.AddConfiguration(config);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("WARNING: appsettings.json embedded resource not found.");
            }

            builder.Services.AddGizmoGoCore(builder.Configuration);

            // localization
            builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");
            builder.Services.AddSingleton<IStringLocalizer, StringLocalizer<UI.Resources>>();
            builder.Services.AddSingleton<AssemblyResourcesLocalizationService>();
            builder.Services.AddSingleton<IAssemblyResourcesLocalizationService>(sp => sp.GetRequiredService<AssemblyResourcesLocalizationService>());
            builder.Services.AddSingleton<ILocalizationService, MauiLocalizationService>();

            // UI services, view states, and view services
            var uiAssembly = typeof(UI.App).Assembly;
            builder.Services.AddUIServices();
            builder.Services.AddViewStates(uiAssembly);
            builder.Services.AddViewServices(uiAssembly);

            // token storage (MAUI SecureStorage)
            builder.Services.AddSingleton<ITokenStorageService, SecureStorageTokenStorageService>();

            // auth infrastructure
            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddSingleton<GoAuthenticationStateProvider>();
            builder.Services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<GoAuthenticationStateProvider>());
            builder.Services.AddTransient<BearerTokenHandler>();
            builder.Services.AddTransient<CultureDelegatingHandler>();

            // provider
            var settings = builder.Configuration.GetSection("GizmoGo").Get<GizmoGoSettings>();

            switch (settings?.Provider)
            {
                case ProviderMode.Platform:
                    builder.Services.AddGizmoGoPlatform();
                    break;
                case ProviderMode.Direct:
                default:
                    builder.Services.AddGizmoGoDirect();
                    break;
            }

            var app = builder.Build();

            // restore persisted culture
            var storedCulture = Preferences.Default.Get<string?>("app.culture", null);
            if (!string.IsNullOrWhiteSpace(storedCulture))
            {
                var culture = new CultureInfo(storedCulture);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }

            // NOTE: Auth session restore and view service initialization happen in
            // App.razor.cs OnInitializedAsync to avoid blocking the main thread
            // during MAUI startup (which can cause deadlocks on Android).

            return app;
        }
    }
}
