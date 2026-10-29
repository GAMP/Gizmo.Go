using Gizmo.Client.UI;
using Gizmo.Go.Core.Models.Agreements;
using Gizmo.Go.Core.Models.PhoneValidation;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Dialog;
using Gizmo.Go.UI.Services.Profile;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.Services.Common
{
    public sealed class UserLoginStatusService : IUserLoginStatusService
    {
        public UserLoginStatusService(
            ILogger<UserLoginStatusService> logger,
            IAuthService authService,
            IPendingUserAgreementsService pendingUserAgreementsService,
            IUserProfileService userProfileService,
            IPhoneValidationService phoneValidationService,
            IGizmoGoDialogService dialogService,
            IProfileCompletionSessionService profileCompletionSession,
            IAuthOptionsService authOptionsService,
            NavigationService navigationService)
        {
            _logger = logger;
            _authService = authService;
            _pendingUserAgreementsService = pendingUserAgreementsService;
            _userProfileService = userProfileService;
            _phoneValidationService = phoneValidationService;
            _dialogService = dialogService;
            _profileCompletionSession = profileCompletionSession;
            _authOptionsService = authOptionsService;
            _navigationService = navigationService;
        }

        private readonly ILogger<UserLoginStatusService> _logger;
        private readonly IAuthService _authService;
        private readonly IPendingUserAgreementsService _pendingUserAgreementsService;
        private readonly IUserProfileService _userProfileService;
        private readonly IPhoneValidationService _phoneValidationService;
        private readonly IGizmoGoDialogService _dialogService;
        private readonly IProfileCompletionSessionService _profileCompletionSession;
        private readonly IAuthOptionsService _authOptionsService;
        private readonly NavigationService _navigationService;

        private bool _isRunning;

        public async Task RunAsync(string? returnUrl, CancellationToken cancellationToken = default)
        {
            if (_isRunning)
                return;

            _isRunning = true;

            try
            {
                var landingUrl = ResolveLandingUrl(returnUrl);

                if (!await ProcessUserAgreementsAsync(cancellationToken))
                    return;

                if (await ProcessProfileAsync(landingUrl, cancellationToken))
                    return;

                if (!IsCurrentLocation(landingUrl))
                    _navigationService.NavigateTo(landingUrl);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Post-login steps failed.");
            }
            finally
            {
                _isRunning = false;
            }
        }

        /// <summary>
        /// Shows the pending agreements one by one.
        /// </summary>
        /// <returns>False when the chain has to stop (a mandatory agreement was declined and the user is logged out, or the dialog was canceled).</returns>
        private async Task<bool> ProcessUserAgreementsAsync(CancellationToken cancellationToken)
        {
            try
            {
                // all unaccepted agreements and the ones that are always shown (IgnoreState)
                IReadOnlyList<PendingUserAgreement> userAgreements = await _pendingUserAgreementsService.GetPendingAsync(cancellationToken);

                foreach (var userAgreement in userAgreements)
                {
                    var agreementPassed = false;

                    var addDialogResult = await _dialogService.ShowUserAgreementDialogAsync(new UserAgreementDialogParameters()
                    {
                        Name = userAgreement.Name ?? string.Empty,
                        Agreement = userAgreement.Agreement ?? string.Empty,
                        IsRejectable = userAgreement.IsRejectable
                    },
                    cancellationToken: cancellationToken);

                    if (addDialogResult.Result != AddComponentResultCode.Opened)
                        continue;

                    // wait for the agreement acceptance result
                    var agreementDialogResult = await addDialogResult.WaitForResultAsync(cancellationToken);

                    if (addDialogResult.Result == AddComponentResultCode.Ok)
                    {
                        if (agreementDialogResult?.Accepted == true)
                        {
                            agreementPassed = true;

                            // an agreement that does not ignore the state is not shown again after the acceptance
                            if (!userAgreement.IgnoreState)
                                await _pendingUserAgreementsService.AcceptAsync(userAgreement.Id, cancellationToken);
                        }
                        else
                        {
                            // declined: passed only if the agreement is rejectable (nothing is written)
                            agreementPassed = userAgreement.IsRejectable;
                        }
                    }
                    else if (addDialogResult.Result == AddComponentResultCode.Dismissed)
                    {
                        agreementPassed = userAgreement.IsRejectable;
                    }
                    else if (addDialogResult.Result == AddComponentResultCode.Canceled)
                    {
                        // canceled only by a global cancellation (logout), nothing else to do
                        return false;
                    }

                    if (!agreementPassed)
                    {
                        // a mandatory agreement was declined: logout (tokens are cleared) and back to Welcome
                        await _authService.LogoutAsync(cancellationToken);
                        _navigationService.NavigateTo(NavigationHelper.WelcomePage);
                        return false;
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Processing user agreements during user login failed.");
            }

            return true;
        }

        /// <summary>
        /// Starts the profile completion step when the user has fields to complete.
        /// </summary>
        /// <returns>True when the profile completion page is opened.</returns>
        private async Task<bool> ProcessProfileAsync(string landingUrl, CancellationToken cancellationToken)
        {
            try
            {
                var isPersonalInfoRequested = _authService.IsPersonalInfoRequested;

                var required = await _userProfileService.GetRequiredInfoAsync(cancellationToken);
                var profile = await _userProfileService.GetAsync(cancellationToken);

                var fields = ProfileCompletionFields.Create(required, profile, isPersonalInfoRequested);
                if (!fields.Any)
                {
                    // the group requires nothing: a stored request would otherwise survive until the group changes
                    if (isPersonalInfoRequested)
                        await _authService.ClearPersonalInfoRequestedAsync(cancellationToken);

                    return false;
                }

                IReadOnlyList<PhoneCountry> phoneCountries = Array.Empty<PhoneCountry>();
                string? defaultPhoneRegionCode = null;
                if (fields.MobilePhone)
                {
                    defaultPhoneRegionCode = (await _authOptionsService.GetAsync(cancellationToken))?.CountryCode;

                    try
                    {
                        phoneCountries = await _phoneValidationService.GetCountriesAsync(cancellationToken);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        // the phone field stays disabled, the other fields can still be completed
                        _logger.LogError(ex, "Cannot load phone countries for profile completion.");
                    }
                }

                // the profile completion page takes its data from the session
                _profileCompletionSession.Start(fields, profile, phoneCountries, defaultPhoneRegionCode, landingUrl);

                if (!IsCurrentLocation(NavigationHelper.ProfileCompletionRoute))
                    _navigationService.NavigateTo(NavigationHelper.ProfileCompletionRoute);

                return true;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Profile completion step failed during login process.");
                return false;
            }
        }

        // the return url wins; otherwise the current route is kept, except the auth entry screens that go to home
        private string ResolveLandingUrl(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Uri.UnescapeDataString(returnUrl);

            var current = GetCurrentLocation();
            var path = GetPath(current);

            if (IsSamePath(path, NavigationHelper.WelcomePage)
                || IsSamePath(path, NavigationHelper.LoginPage)
                || IsSamePath(path, NavigationHelper.ProfileCompletionRoute))
            {
                return NavigationHelper.HomePage;
            }

            return current;
        }

        // route with query relative to the app base
        private string GetCurrentLocation()
        {
            var uri = _navigationService.GetUri();
            var baseUri = _navigationService.GetBaseUri();

            var relative = !string.IsNullOrEmpty(baseUri) && uri.StartsWith(baseUri, StringComparison.OrdinalIgnoreCase)
                ? uri[baseUri.Length..]
                : uri;

            return "/" + relative.TrimStart('/');
        }

        private static string GetPath(string location)
        {
            var end = location.IndexOfAny(['?', '#']);
            return "/" + (end >= 0 ? location[..end] : location).Trim('/');
        }

        private static bool IsSamePath(string path, string route) =>
            string.Equals(path, "/" + route.Trim('/'), StringComparison.OrdinalIgnoreCase);

        private bool IsCurrentLocation(string url) =>
            string.Equals("/" + url.TrimStart('/'), GetCurrentLocation(), StringComparison.OrdinalIgnoreCase);
    }
}
