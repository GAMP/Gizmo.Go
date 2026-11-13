using System.Globalization;
using Gizmo.Go.Core.Models.Auth;
using Gizmo.Go.Core.Models.PhoneValidation;
using Gizmo.Go.Core.Models.Profile;
using Gizmo.Go.Core.Models.Notification;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Core.Services.Notification;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Common;
using Gizmo.Go.UI.Services.Dialog;
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
    /// Profile screen: the profile header, the settings rows, the notification switches and the logout with a confirmation.
    /// </summary>
    [Register()]
    [Route(NavigationHelper.ProfileRoute)]
    public sealed class ProfileViewService : ViewStateServiceBase<ProfileViewState>
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ILadderService _ladderService;
        private readonly IPhoneValidationService _phoneValidationService;
        private readonly IAuthService _authService;
        private readonly IDeviceSettingsService _deviceSettingsService;
        private readonly IGizmoGoDialogService _dialogService;
        private readonly IGizmoGoNotificationsService _notificationsService;
        private readonly NavigationService _navigationService;
        private readonly ILocalizationService _localizationService;

        private CancellationTokenSource? _loadCts;

        public ProfileViewService(
            ProfileViewState viewState,
            ILogger<ProfileViewService> logger,
            IServiceProvider serviceProvider,
            IUserProfileService userProfileService,
            ILadderService ladderService,
            IPhoneValidationService phoneValidationService,
            IAuthService authService,
            IDeviceSettingsService deviceSettingsService,
            IGizmoGoDialogService dialogService,
            IGizmoGoNotificationsService notificationsService,
            NavigationService navigationService,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _userProfileService = userProfileService;
            _ladderService = ladderService;
            _phoneValidationService = phoneValidationService;
            _authService = authService;
            _deviceSettingsService = deviceSettingsService;
            _dialogService = dialogService;
            _notificationsService = notificationsService;
            _navigationService = navigationService;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Loads the profile when the screen was opened without a navigation event (a direct url).
        /// </summary>
        public Task EnsureLoadedAsync() => _loadCts is null ? LoadAsync() : Task.CompletedTask;

        public async Task LoadAsync()
        {
            CancelLoad();

            var cts = _loadCts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            ViewState.Profile.Reset();
            ViewState.Level.Reset();
            ViewState.MaskedPhone = null;
            ViewState.SinceText = null;
            ViewState.LanguageName = GetLanguageName();
            RaiseViewStateChanged();

            await Task.WhenAll(
                LoadSettingsAsync(),
                LoadProfileAsync(cancellationToken),
                ViewState.Level.LoadAsync(ct => _ladderService.GetLevelAsync(ct), Logger, "ladder level", DebounceViewStateChanged, cancellationToken));

            if (!cancellationToken.IsCancellationRequested)
                RaiseViewStateChanged();
        }

        /// <summary>
        /// Returns to the previous screen; an entry opened directly (without an app history) opens Home.
        /// </summary>
        /// <param name="historyEntryState">History entry state of the current screen.</param>
        public Task BackAsync(string? historyEntryState) => _navigationService.GoBackOrNavigateAsync(historyEntryState, NavigationHelper.HomePage);

        public void OpenLanguage() => _navigationService.NavigateTo(NavigationHelper.ProfileLanguageRoute, NavigationHelper.InAppNavigation);

        public void OpenPolicies() => _navigationService.NavigateTo(NavigationHelper.ProfilePoliciesRoute, NavigationHelper.InAppNavigation);

        public void OpenChangePassword() => _navigationService.NavigateTo(NavigationHelper.ProfileChangePasswordRoute, NavigationHelper.InAppNavigation);

        public void OpenAbout() => _navigationService.NavigateTo(NavigationHelper.ProfileAboutRoute, NavigationHelper.InAppNavigation);

        /// <summary>
        /// Switches the notifications about the own reservations and balance; the value is stored on the device.
        /// </summary>
        public Task TogglePersonalNotificationsAsync() => ToggleNotificationAsync(personal: true);

        /// <summary>
        /// Switches the notifications about published news; the value is stored on the device.
        /// </summary>
        public Task ToggleNewsNotificationsAsync() => ToggleNotificationAsync(personal: false);

        public async Task OpenLadderInfoAsync()
        {
            try
            {
                await _dialogService.ShowLadderInfoDialogAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot show the ladder info dialog.");
            }
        }

        /// <summary>
        /// Asks for a confirmation, then drops the session and opens Welcome. Works without a loaded profile.
        /// </summary>
        public async Task LogoutAsync()
        {
            try
            {
                var addDialogResult = await _dialogService.ShowConfirmDialogAsync(
                    _localizationService.GetString(nameof(Autogenerated.Resources.GO_PROFILE_LOGOUT_CONFIRM_TITLE)),
                    _localizationService.GetString(nameof(Autogenerated.Resources.GO_PROFILE_LOGOUT_CONFIRM_TEXT)),
                    _localizationService.GetString(nameof(Autogenerated.Resources.GO_PROFILE_LOGOUT_CONFIRM_BUTTON)));

                if (addDialogResult.Result != AddComponentResultCode.Opened)
                    return;

                await addDialogResult.WaitForResultAsync();

                // "Cancel", a close by an overlay click and a global cancellation keep the session
                if (addDialogResult.Result != AddComponentResultCode.Ok)
                    return;

                await _authService.LogoutAsync();

                // Welcome replaces the profile entry: a back step does not return to a protected screen
                _navigationService.NavigateTo(NavigationHelper.WelcomePage, new NavigationOptions { ReplaceHistoryEntry = true });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot log out.");
            }
        }

        /// <summary>
        /// Asks for a confirmation, then deletes the account on the server, drops the session and opens Welcome.
        /// A failure keeps the session and shows a notification.
        /// </summary>
        public async Task DeleteAccountAsync()
        {
            try
            {
                var addDialogResult = await _dialogService.ShowConfirmDialogAsync(
                    _localizationService.GetString(nameof(Autogenerated.Resources.GO_PROFILE_DELETE_CONFIRM_TITLE)),
                    _localizationService.GetString(nameof(Autogenerated.Resources.GO_PROFILE_DELETE_CONFIRM_TEXT)),
                    _localizationService.GetString(nameof(Autogenerated.Resources.GO_PROFILE_DELETE_CONFIRM_BUTTON)),
                    isDanger: true);

                if (addDialogResult.Result != AddComponentResultCode.Opened)
                    return;

                await addDialogResult.WaitForResultAsync();

                // "Cancel", a close by an overlay click and a global cancellation keep the account
                if (addDialogResult.Result != AddComponentResultCode.Ok)
                    return;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot show the delete account dialog.");
                return;
            }

            try
            {
                await _userProfileService.DeleteAsync();
                await _authService.LogoutAsync();

                // Welcome replaces the profile entry: a back step does not return to a protected screen
                _navigationService.NavigateTo(NavigationHelper.WelcomePage, new NavigationOptions { ReplaceHistoryEntry = true });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot delete the account.");
                await _notificationsService.ShowAsync(_localizationService.GetString(nameof(Autogenerated.Resources.GO_GEN_AN_ERROR_HAS_OCCURRED)), GoNotificationType.Error);
            }
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            // an unauthenticated user is redirected to Welcome by the page, the secured calls would only fail
            if (_authService.State != AuthState.Authenticated)
                return Task.CompletedTask;

            return LoadAsync();
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            CancelLoad();
            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        private void CancelLoad()
        {
            _loadCts?.Cancel();
            _loadCts = null;
        }

        private async Task LoadSettingsAsync()
        {
            var settings = await _deviceSettingsService.GetAsync();
            ViewState.PersonalNotifications = settings.PersonalNotifications;
            ViewState.NewsNotifications = settings.NewsNotifications;
            DebounceViewStateChanged();
        }

        // the switch moves at once; a failed write is logged by the storage and the value stays on the screen
        private async Task ToggleNotificationAsync(bool personal)
        {
            try
            {
                var value = personal ? !ViewState.PersonalNotifications : !ViewState.NewsNotifications;

                if (personal)
                    ViewState.PersonalNotifications = value;
                else
                    ViewState.NewsNotifications = value;

                RaiseViewStateChanged();

                // read-modify-write: the language screen stores its value in the same settings
                var settings = await _deviceSettingsService.GetAsync();

                if (personal)
                    settings.PersonalNotifications = value;
                else
                    settings.NewsNotifications = value;

                await _deviceSettingsService.SetAsync(settings);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot store the notification switch.");
            }
        }

        private async Task LoadProfileAsync(CancellationToken cancellationToken)
        {
            string? maskedPhone = null;

            // the phone is masked inside the block load, so the header is shown at once with its phone line
            await ViewState.Profile.LoadAsync(async ct =>
            {
                var details = await _userProfileService.GetAsync(ct);
                maskedPhone = await GetMaskedPhoneAsync(details.MobilePhone, ct);
                return details;
            }, Logger, "profile", DebounceViewStateChanged, cancellationToken);

            if (cancellationToken.IsCancellationRequested || ViewState.Profile.Value is not { } profile)
                return;

            ViewState.Nickname = profile.Username;
            ViewState.Initials = GetInitials(profile);
            ViewState.MaskedPhone = maskedPhone;
            ViewState.SinceText = profile.RegisteredAt is { } registeredAt
                ? _localizationService.GetString(nameof(Autogenerated.Resources.GO_PROFILE_SINCE), registeredAt.ToString("MMMM yyyy", CultureInfo.CurrentCulture))
                : null;

            DebounceViewStateChanged();
        }

        // the countries give the calling codes; without them the phone is never shown unmasked (null, the line is hidden)
        private async Task<string?> GetMaskedPhoneAsync(string? mobilePhone, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(mobilePhone))
                return null;

            try
            {
                IReadOnlyList<PhoneCountry> countries = await _phoneValidationService.GetCountriesAsync(cancellationToken);
                return ContactMasking.MaskPhone(mobilePhone, countries);
            }
            catch (Exception ex) when (ex is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
            {
                Logger.LogError(ex, "Cannot load the phone countries to mask the profile phone.");
                return null;
            }
        }

        private static string GetInitials(UserProfileDetails profile)
        {
            var first = profile.FirstName?.Trim();
            var last = profile.LastName?.Trim();

            if (!string.IsNullOrEmpty(first) && !string.IsNullOrEmpty(last))
                return string.Concat(first[0], last[0]).ToUpper(CultureInfo.CurrentCulture);

            var username = profile.Username;
            return (username.Length > 2 ? username[..2] : username).ToUpper(CultureInfo.CurrentCulture);
        }

        private static string GetLanguageName() => AppLanguages.GetNativeName(CultureInfo.CurrentUICulture);
    }
}
