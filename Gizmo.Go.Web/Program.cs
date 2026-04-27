using System.Globalization;
using Gizmo;
using Gizmo.Go.Core.Configuration;
using Gizmo.Go.Core.Extensions;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Extensions;
using Gizmo.Go.Provider.Platform.Extensions;
using Gizmo.Go.UI;
using Gizmo.Go.UI.Extensions;
using Gizmo.Go.UI.Providers;
using Gizmo.Go.Web.Services;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddGizmoGoCore(builder.Configuration);

// localization
builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");
builder.Services.AddSingleton<IStringLocalizer, StringLocalizer<Gizmo.Go.UI.Resources>>();
builder.Services.AddSingleton<AssemblyResourcesLocalizationService>();
builder.Services.AddSingleton<IAssemblyResourcesLocalizationService>(sp => sp.GetRequiredService<AssemblyResourcesLocalizationService>());
builder.Services.AddSingleton<ILocalizationService, GoLocalizationService>();

// UI services, view states, and view services
var uiAssembly = typeof(App).Assembly;
builder.Services.AddUIServices();
builder.Services.AddViewStates(uiAssembly);
builder.Services.AddViewServices(uiAssembly);

// token storage
builder.Services.AddSingleton<ITokenStorageService, LocalStorageTokenStorageService>();
builder.Services.AddGizmoGoUI();

// external launcher
builder.Services.AddSingleton<IExternalLauncher, WebExternalLauncher>();

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

// lifecycle service
builder.Services.AddSingleton<WebAppLifecycleService>();
builder.Services.AddSingleton<IAppLifecycleService>(sp =>
    sp.GetRequiredService<WebAppLifecycleService>());

var host = builder.Build();

// restore persisted culture from localStorage
var js = host.Services.GetRequiredService<IJSRuntime>();
var storedCulture = await js.InvokeAsync<string?>("localStorage.getItem", "app.culture");
if (!string.IsNullOrWhiteSpace(storedCulture))
{
    var culture = new CultureInfo(storedCulture);
    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}

// restore persisted auth session (before first render to avoid login flash)
await host.Services.GetRequiredService<IAuthService>().TryRestoreSessionAsync();

// initialize all view services (sets up EditContext subscriptions, navigation lifecycle, etc.)
await host.Services.InitializeViewsServices();

await host.RunAsync();
