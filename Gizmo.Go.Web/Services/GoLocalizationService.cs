using System.Globalization;
using Gizmo;
using Gizmo.UI.Services;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.Web.Services
{
    /// <summary>
    /// Gizmo Go web localization service.
    /// </summary>
    public sealed class GoLocalizationService : LocalizationServiceBase
    {
        private readonly IAssemblyResourcesLocalizationService _assemblyResourcesLocalizationService;

        public GoLocalizationService(
            ILogger<GoLocalizationService> logger,
            IStringLocalizer localizer,
            IAssemblyResourcesLocalizationService assemblyResourcesLocalizationService) : base(logger, localizer)
        {
            _assemblyResourcesLocalizationService = assemblyResourcesLocalizationService;
        }

        public override event EventHandler<EventArgs>? LocalizationOptionsChanged;
        public override event EventHandler<EventArgs>? LanguageChanged;

        // the culture is not persisted: the app always starts with the server culture
        public override Task SetCurrentCultureAsync(CultureInfo culture)
        {
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
