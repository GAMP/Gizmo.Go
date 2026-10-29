using System.Web;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Common;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages
{
    [Register()]
    [Route(NavigationHelper.LoginPage)]
    public sealed class LoginViewService : ValidatingViewStateServiceBase<LoginViewState>
    {
        private readonly IAuthService _authService;
        private readonly IPhoneValidationService _phoneValidationService;
        private readonly NavigationService _navigationService;
        private readonly IAssemblyResourcesLocalizationService _assemblyResourcesLocalizationService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserLoginStatusService _userLoginStatusService;
        private readonly IAuthOptionsService _authOptionsService;
        private string? _returnUrl;

        public LoginViewService(
            LoginViewState viewState,
            ILogger<LoginViewService> logger,
            IServiceProvider serviceProvider,
            IAuthService authService,
            IPhoneValidationService phoneValidationService,
            NavigationService navigationService,
            IAssemblyResourcesLocalizationService assemblyResourcesLocalizationService,
            ILocalizationService localizationService,
            IUserLoginStatusService userLoginStatusService,
            IAuthOptionsService authOptionsService) : base(viewState, logger, serviceProvider)
        {
            _authService = authService;
            _phoneValidationService = phoneValidationService;
            _navigationService = navigationService;
            _assemblyResourcesLocalizationService = assemblyResourcesLocalizationService;
            _localizationService = localizationService;
            _userLoginStatusService = userLoginStatusService;
            _authOptionsService = authOptionsService;
        }

        /// <summary>
        /// Loads the server options of the screen. Called by the page on every show (also on the app start, when the
        /// first navigation does not reach the view services); a failed load is retried on the next show.
        /// </summary>
        public async Task LoadOptionsAsync(CancellationToken cancellationToken = default)
        {
            var options = await _authOptionsService.GetAsync(cancellationToken);

            ViewState.HasOptionsError = options is null;
            ViewState.IsRegistrationEnabled = options?.IsRegistrationEnabled ?? false;
            ViewState.IsPasswordRecoveryEnabled = options?.IsPasswordRecoveryEnabled ?? false;
            ViewState.LogoUrl = options?.LogoUrl;
            ViewState.DefaultPhoneRegionCode = options?.CountryCode;
            ViewState.RaiseChanged();
        }

        // phone tab with a number: SetPhoneAsync keeps the digits in both Phone and Username
        private bool IsPhoneIdentifier =>
            !string.IsNullOrEmpty(ViewState.Phone) && string.Equals(ViewState.Username, ViewState.Phone, StringComparison.Ordinal);

        public ValueTask SetUsernameAsync(string value)
        {
            ViewState.Username = value;
            ViewState.ErrorMessage = null;
            ValidateProperty(() => ViewState.Username);
            return ValueTask.CompletedTask;
        }

        public void SetPhoneRegionCode(string? regionCode)
        {
            ViewState.PhoneRegionCode = regionCode;
        }

        /// <summary>
        /// Stores the phone field value (calling code digits followed by national digits).
        /// The login identifier is set only when there are national digits.
        /// </summary>
        public ValueTask SetPhoneAsync(string digits)
        {
            var callingCode = ViewState.PhoneCountries
                .FirstOrDefault(c => string.Equals(c.RegionCode, ViewState.PhoneRegionCode, StringComparison.OrdinalIgnoreCase))
                ?.CallingCode;
            var codeDigits = new string((callingCode ?? string.Empty).Where(char.IsDigit).ToArray());

            ViewState.Phone = digits;
            ViewState.Username = digits.Length > codeDigits.Length && digits.StartsWith(codeDigits, StringComparison.Ordinal)
                ? digits
                : string.Empty;
            ViewState.ErrorMessage = null;

            // a calling code alone (country just selected) is not an input yet: no "required" message
            if (string.IsNullOrEmpty(ViewState.Username))
            {
                ClearError(() => ViewState.Username);
                ViewState.RaiseChanged();
            }
            else
            {
                ValidateProperty(() => ViewState.Username);
            }

            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Resets the login identifier when the login tab changes, so a phone is never sent as a nickname and vice versa.
        /// </summary>
        public void ClearIdentifier()
        {
            ViewState.Username = string.Empty;
            ViewState.Phone = string.Empty;
            ViewState.ErrorMessage = null;
            ClearError(() => ViewState.Username);
            ViewState.RaiseChanged();
        }

        public ValueTask SetPasswordAsync(string value)
        {
            ViewState.Password = value;
            ViewState.ErrorMessage = null;
            ValidateProperty(() => ViewState.Password);
            return ValueTask.CompletedTask;
        }

        public async ValueTask SubmitAsync(CancellationToken cancellationToken = default)
        {
            // the login and the post-login steps are in progress
            if (ViewState.IsLoading)
                return;

            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.ErrorMessage = null;
            ViewState.RaiseChanged();

            var isLoggedIn = false;

            try
            {
                var result = await _authService.LoginAsync(ViewState.Username, ViewState.Password, cancellationToken);

                if (result.Success)
                {
                    isLoggedIn = true;
                }
                else if (result.ErrorCode.HasValue)
                {
                    ViewState.ErrorMessage = _assemblyResourcesLocalizationService.GetLocalizedStringValueOrName(result.ErrorCode.Value);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Login failed.");
                ViewState.ErrorMessage = _assemblyResourcesLocalizationService.GetLocalizedStringValueOrName(AuthErrorCode.Unexpected);
            }
            finally
            {
                // after a successful login the form stays disabled until the post-login steps are done
                if (!isLoggedIn)
                {
                    ViewState.IsLoading = false;
                    ViewState.RaiseChanged();
                }
            }

            if (!isLoggedIn)
                return;

            try
            {
                // pending agreements, profile completion, then the return url or home
                await _userLoginStatusService.RunAsync(_returnUrl, cancellationToken);
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public ValueTask NavigateToCreateAccountAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationIndexRoute);
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateToPasswordRecoveryAsync()
        {
            // the Kind step clears the recovery session left from a previous attempt (as the client Login link)
            _navigationService.NavigateTo(NavigationHelper.PasswordRecoveryKindRoute);
            return ValueTask.CompletedTask;
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ViewState.Username = string.Empty;
            ViewState.Password = string.Empty;
            ViewState.Phone = string.Empty;
            ViewState.PhoneRegionCode = null;
            ViewState.IsLoading = false;
            ViewState.ErrorMessage = null;

            var currentUri = _navigationService.GetUri();
            if (!string.IsNullOrEmpty(currentUri))
            {
                var uri = new Uri(currentUri);
                _returnUrl = HttpUtility.ParseQueryString(uri.Query).Get("returnUrl");
            }

            // the default phone country has to be known before the countries reach the phone field
            await LoadOptionsAsync(cancellationToken);

            try
            {
                ViewState.PhoneCountries = await _phoneValidationService.GetCountriesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // the phone field stays disabled, the nickname tab still works
                Logger.LogError(ex, "Cannot load phone countries for login.");
                ViewState.PhoneCountries = Array.Empty<Gizmo.Go.Core.Models.PhoneValidation.PhoneCountry>();
            }

            ViewState.RaiseChanged();

            await base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        // the phone number format is checked as on the registration phone step; a nickname has no async check
        protected override async Task<IEnumerable<string>> OnValidateAsync(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger, CancellationToken cancellationToken = default)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.Username) && IsPhoneIdentifier)
            {
                if (string.IsNullOrEmpty(ViewState.PhoneRegionCode))
                    return new[] { _localizationService.GetString(nameof(Autogenerated.Resources.GO_REGISTRATION_VE_SELECT_COUNTRY)) };

                try
                {
                    var result = await _phoneValidationService.ValidateAsync(ViewState.Username, ViewState.PhoneRegionCode, cancellationToken);
                    if (!result.IsValid)
                        return new[] { _localizationService.GetString(result.ErrorKey ?? nameof(Autogenerated.Resources.GO_GEN_VE_INVALID_FIELD)) };
                }
                catch (Exception ex)
                {
                    // the server still checks the identifier on login
                    Logger.LogError(ex, "Cannot validate login phone.");
                }
            }

            return await base.OnValidateAsync(fieldIdentifier, validationTrigger, cancellationToken);
        }
    }
}
