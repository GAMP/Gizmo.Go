using System.Globalization;
using System.Reflection;
using Gizmo;
using Gizmo.Go.Core.Configuration;
using Gizmo.Go.Core.Extensions;
using Gizmo.Go.Provider.Direct.Extensions;
using Gizmo.Go.Provider.Platform.Extensions;
using Gizmo.Go.Web;
using Gizmo.Go.Web.Providers;
using Gizmo.Go.Web.Services;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddGizmoGoCore(builder.Configuration);

// localization
builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");
builder.Services.AddSingleton<IStringLocalizer, StringLocalizer<Resources>>();
builder.Services.AddSingleton<AssemblyResourcesLocalizationService>();
builder.Services.AddSingleton<IAssemblyResourcesLocalizationService>(sp => sp.GetRequiredService<AssemblyResourcesLocalizationService>());
builder.Services.AddSingleton<ILocalizationService, GoLocalizationService>();

// UI services, view states, and view services
var webAssembly = Assembly.GetExecutingAssembly();
builder.Services.AddUIServices();
builder.Services.AddViewStates(webAssembly);
builder.Services.AddViewServices(webAssembly);

// auth infrastructure
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<GoAuthenticationStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<GoAuthenticationStateProvider>());
builder.Services.AddSingleton<IAccessTokenProvider>(sp => sp.GetRequiredService<GoAuthenticationStateProvider>());
builder.Services.AddTransient<GoAuthorizationMessageHandler>();
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

// initialize all view services (sets up EditContext subscriptions, navigation lifecycle, etc.)
await host.Services.InitializeViewsServices();

await host.RunAsync();
