using System.Globalization;
using Gizmo;
using Gizmo.UI.Services;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.Maui.Services
{
    /// <summary>
    /// Gizmo Go MAUI localization service.
    /// </summary>
    public sealed class MauiLocalizationService : LocalizationServiceBase
    {
        private readonly IAssemblyResourcesLocalizationService _assemblyResourcesLocalizationService;

        public MauiLocalizationService(
            ILogger<MauiLocalizationService> logger,
            IStringLocalizer localizer,
            IAssemblyResourcesLocalizationService assemblyResourcesLocalizationService) : base(logger, localizer)
        {
            _assemblyResourcesLocalizationService = assemblyResourcesLocalizationService;
        }

        public override event EventHandler<EventArgs>? LocalizationOptionsChanged;
        public override event EventHandler<EventArgs>? LanguageChanged;

        public override Task SetCurrentCultureAsync(CultureInfo culture)
        {
            try
            {
                Preferences.Default.Set("app.culture", culture.Name);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Error setting MAUI preferences based culture.");
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;

            _assemblyResourcesLocalizationService.SetCulture(culture);

            LanguageChanged?.Invoke(this, EventArgs.Empty);

            return Task.CompletedTask;
        }

        protected override void ConfigureLocalizationOptions(IEnumerable<CultureInfo> cultures)
        {
            LocalizationOptionsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
