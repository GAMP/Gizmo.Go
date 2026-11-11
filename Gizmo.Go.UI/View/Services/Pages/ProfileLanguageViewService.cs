using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages
{
    /// <summary>
    /// Language screen: the app languages and the switch with the choice stored on the device.
    /// </summary>
    [Register()]
    [Route(NavigationHelper.ProfileLanguageRoute)]
    public sealed class ProfileLanguageViewService : ViewStateServiceBase<ProfileLanguageViewState>
    {
        private readonly IDeviceSettingsService _deviceSettingsService;
        private readonly IMobileOptionsService _mobileOptionsService;
        private readonly ILocalizationService _localizationService;
        private readonly NavigationService _navigationService;

        public ProfileLanguageViewService(
            ProfileLanguageViewState viewState,
            ILogger<ProfileLanguageViewService> logger,
            IServiceProvider serviceProvider,
            IDeviceSettingsService deviceSettingsService,
            IMobileOptionsService mobileOptionsService,
            ILocalizationService localizationService,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _deviceSettingsService = deviceSettingsService;
            _mobileOptionsService = mobileOptionsService;
            _localizationService = localizationService;
            _navigationService = navigationService;
        }

        /// <summary>
        /// Builds the language list and finds the current language (also called by the page for a screen opened without a navigation event).
        /// </summary>
        public void Load()
        {
            ViewState.Languages = AppLanguages.GetAll(Logger);
            ViewState.CurrentCode = AppLanguages.FindCurrentCode(ViewState.Languages);
            ViewState.IsSwitching = false;
            RaiseViewStateChanged();
        }

        /// <summary>
        /// Returns to the previous screen; an entry opened directly (without an app history) opens the profile.
        /// </summary>
        /// <param name="historyEntryState">History entry state of the current screen.</param>
        public Task BackAsync(string? historyEntryState) => _navigationService.GoBackOrNavigateAsync(historyEntryState, NavigationHelper.ProfileRoute);

        /// <summary>
        /// Stores the chosen language and reloads the app on the profile in it. The current language only goes back.
        /// </summary>
        /// <param name="code">Culture name of the chosen language.</param>
        /// <param name="historyEntryState">History entry state of the current screen.</param>
        public async Task SelectAsync(string code, string? historyEntryState)
        {
            if (ViewState.IsSwitching)
                return;

            if (code == ViewState.CurrentCode)
            {
                await BackAsync(historyEntryState);
                return;
            }

            var previousCode = ViewState.CurrentCode;
            ViewState.IsSwitching = true;
            ViewState.CurrentCode = code;
            RaiseViewStateChanged();

            try
            {
                var settings = await _deviceSettingsService.GetAsync();
                settings.Language = code;
                await _deviceSettingsService.SetAsync(settings);

                // Web: the satellite resource assemblies of the new culture are loaded only at the start (Program.cs applies the
                // stored language), so the reload below does the switch; MAUI has all resources: switch the running app first
                if (!OperatingSystem.IsBrowser())
                {
                    var culture = await CultureHelper.CreateCultureAsync(_mobileOptionsService, code, Logger);

                    if (culture is not null)
                        await _localizationService.SetCurrentCultureAsync(culture);
                }

                _navigationService.NavigateTo(NavigationHelper.ProfileRoute, new NavigationOptions { ForceLoad = true, ReplaceHistoryEntry = true });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot change the language to {Language}.", code);
                ViewState.CurrentCode = previousCode;
                ViewState.IsSwitching = false;
                RaiseViewStateChanged();
            }
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            Load();
            return Task.CompletedTask;
        }
    }
}
