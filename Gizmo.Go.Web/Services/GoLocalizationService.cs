using System.Globalization;
using Gizmo;
using Gizmo.UI.Services;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Gizmo.Go.Web.Services
{
    /// <summary>
    /// Gizmo Go web localization service.
    /// </summary>
    public sealed class GoLocalizationService : LocalizationServiceBase
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly IAssemblyResourcesLocalizationService _assemblyResourcesLocalizationService;

        public GoLocalizationService(
            ILogger<GoLocalizationService> logger,
            IStringLocalizer localizer,
            IJSRuntime jsRuntime,
            IAssemblyResourcesLocalizationService assemblyResourcesLocalizationService) : base(logger, localizer)
        {
            _jsRuntime = jsRuntime;
            _assemblyResourcesLocalizationService = assemblyResourcesLocalizationService;
        }

        public override event EventHandler<EventArgs>? LocalizationOptionsChanged;
        public override event EventHandler<EventArgs>? LanguageChanged;

        public override async Task SetCurrentCultureAsync(CultureInfo culture)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "app.culture", culture.Name);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Error setting browser based culture.");
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;

            _assemblyResourcesLocalizationService.SetCulture(culture);

            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void ConfigureLocalizationOptions(IEnumerable<CultureInfo> cultures)
        {
            LocalizationOptionsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
