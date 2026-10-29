using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Models.Profile;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Profile;
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
    /// <summary>
    /// Completion of the current user profile after login, after the client UserChangeProfileViewService.
    /// The fields to show, the loaded profile and the landing url come from <see cref="IProfileCompletionSessionService"/>,
    /// started by the post-login steps (<see cref="ProfileCompletionFields"/> describes both modes).
    /// </summary>
    [Register()]
    [Route(NavigationHelper.ProfileCompletionRoute)]
    public sealed class ProfileCompletionViewService : ValidatingViewStateServiceBase<ProfileCompletionViewState>
    {
        public ProfileCompletionViewService(ProfileCompletionViewState viewState,
            ILogger<ProfileCompletionViewService> logger,
            IServiceProvider serviceProvider,
            IAuthService authService,
            IUserProfileService userProfileService,
            IUserRegistrationService registrationService,
            IPhoneValidationService phoneValidationService,
            ILocalizationService localizationService,
            IProfileCompletionSessionService profileCompletionSession) : base(viewState, logger, serviceProvider)
        {
            _authService = authService;
            _userProfileService = userProfileService;
            _registrationService = registrationService;
            _phoneValidationService = phoneValidationService;
            _localizationService = localizationService;
            _profileCompletionSession = profileCompletionSession;
        }

        private const int MIN_NATIONAL_PHONE_LENGTH = 7;

        private readonly IAuthService _authService;
        private readonly IUserProfileService _userProfileService;
        private readonly IUserRegistrationService _registrationService;
        private readonly IPhoneValidationService _phoneValidationService;
        private readonly ILocalizationService _localizationService;
        private readonly IProfileCompletionSessionService _profileCompletionSession;

        private string? _mobileE164;

        public void SetFirstName(string? value)
        {
            ViewState.FirstName = value;
            ValidateProperty(() => ViewState.FirstName);
        }

        public void SetLastName(string? value)
        {
            ViewState.LastName = value;
            ValidateProperty(() => ViewState.LastName);
        }

        public void SetBirthDate(DateTime? value)
        {
            ViewState.BirthDate = value;
            ValidateProperty(() => ViewState.BirthDate);
        }

        public void SetSex(UserSex value)
        {
            ViewState.Sex = value;
            ValidateProperty(() => ViewState.Sex);
        }

        public void SetEmail(string? value)
        {
            ViewState.Email = value;
            ValidateProperty(() => ViewState.Email);
        }

        public void SetMobilePhone(string? value)
        {
            ViewState.MobilePhone = value;
            _mobileE164 = null;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void SetPhoneRegionCode(string? value)
        {
            ViewState.PhoneRegionCode = value;
            _mobileE164 = null;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void SetPhone(string? value)
        {
            ViewState.Phone = value;
            ValidateProperty(() => ViewState.Phone);
        }

        public void SetAddress(string? value)
        {
            ViewState.Address = value;
            ValidateProperty(() => ViewState.Address);
        }

        public void SetCity(string? value)
        {
            ViewState.City = value;
            ValidateProperty(() => ViewState.City);
        }

        public void SetPostCode(string? value)
        {
            ViewState.PostCode = value;
            ValidateProperty(() => ViewState.PostCode);
        }

        public void SetCountry(string? value)
        {
            ViewState.Country = value;
            ValidateProperty(() => ViewState.Country);
        }

        public async Task SubmitAsync()
        {
            if (ViewState.IsLoading)
                return;

            Validate();

            if (ViewState.IsValid != true)
                return;

            if (ViewState.ShowMobilePhone && string.IsNullOrEmpty(_mobileE164))
            {
                ValidateProperty(() => ViewState.MobilePhone);
                return;
            }

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            if (!await ValidateAvailabilityAsync())
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
                return;
            }

            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            try
            {
                var result = await _userProfileService.UpdateAsync(new UserProfileUpdate()
                {
                    FirstName = ViewState.ShowFirstName ? ViewState.FirstName ?? string.Empty : null,
                    LastName = ViewState.ShowLastName ? ViewState.LastName ?? string.Empty : null,
                    BirthDate = ViewState.ShowBirthDate ? ViewState.BirthDate : null,
                    Sex = ViewState.ShowSex ? ViewState.Sex : null,
                    Email = ViewState.ShowEmail ? ViewState.Email ?? string.Empty : null,
                    MobilePhone = ViewState.ShowMobilePhone ? _mobileE164 : null,
                    Phone = ViewState.ShowPhone ? ViewState.Phone ?? string.Empty : null,
                    Address = ViewState.ShowAddress ? ViewState.Address ?? string.Empty : null,
                    City = ViewState.ShowCity ? ViewState.City ?? string.Empty : null,
                    PostCode = ViewState.ShowPostCode ? ViewState.PostCode ?? string.Empty : null,
                    Country = ViewState.ShowCountry ? ViewState.Country ?? string.Empty : null,
                });

                switch (result)
                {
                    case UserProfileUpdateCode.Success:
                        await _authService.ClearPersonalInfoRequestedAsync();
                        var landingUrl = _profileCompletionSession.LandingUrl;
                        _profileCompletionSession.Clear();
                        NavigationService.NavigateTo(string.IsNullOrEmpty(landingUrl) ? NavigationHelper.HomePage : landingUrl);
                        break;
                    case UserProfileUpdateCode.NonUniqueValue:
                        // the availability check shows which of the values is taken
                        if (ViewState.ShowEmail)
                            ValidateProperty(() => ViewState.Email);
                        if (ViewState.ShowMobilePhone)
                            ValidateProperty(() => ViewState.MobilePhone);
                        SetError(nameof(Autogenerated.Resources.GO_PROFILE_COMPLETION_NON_UNIQUE));
                        break;
                    case UserProfileUpdateCode.InvalidInput:
                        SetError(nameof(Autogenerated.Resources.GO_PROFILE_COMPLETION_INVALID_INPUT));
                        break;
                    default:
                        SetError(nameof(Autogenerated.Resources.GO_GEN_AN_ERROR_HAS_OCCURRED));
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User profile update error.");
                SetError(nameof(Autogenerated.Resources.GO_GEN_AN_ERROR_HAS_OCCURRED));
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _profileCompletionSession.Changed += OnProfileCompletionSessionChanged;

            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _profileCompletionSession.Changed -= OnProfileCompletionSessionChanged;

            base.OnDisposing(isDisposing);
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            // the page is opened only by the post-login steps; a direct entry has no form and no way back
            if (!_profileCompletionSession.IsActive)
                NavigationService.NavigateTo(NavigationHelper.HomePage);

            return Task.CompletedTask;
        }

        // the session is started by the post-login steps, also while this page is already open
        private void OnProfileCompletionSessionChanged(object? sender, EventArgs e)
        {
            Reset();

            if (!_profileCompletionSession.IsActive)
                return;

            var fields = _profileCompletionSession.Fields;
            var profile = _profileCompletionSession.Profile!;

            ViewState.ShowFirstName = fields.FirstName;
            ViewState.ShowLastName = fields.LastName;
            ViewState.ShowBirthDate = fields.BirthDate;
            ViewState.ShowSex = fields.Sex;
            ViewState.ShowEmail = fields.Email;
            ViewState.ShowMobilePhone = fields.MobilePhone;
            ViewState.ShowPhone = fields.Phone;
            ViewState.ShowAddress = fields.Address;
            ViewState.ShowCity = fields.City;
            ViewState.ShowPostCode = fields.PostCode;
            ViewState.ShowCountry = fields.Country;

            ViewState.FirstName = fields.FirstName ? profile.FirstName : null;
            ViewState.LastName = fields.LastName ? profile.LastName : null;
            ViewState.BirthDate = fields.BirthDate ? profile.BirthDate : null;
            ViewState.Sex = fields.Sex ? profile.Sex : UserSex.Unspecified;
            ViewState.Email = fields.Email ? profile.Email : null;
            ViewState.MobilePhone = fields.MobilePhone ? profile.MobilePhone?.TrimStart('+') : null;
            ViewState.Phone = fields.Phone ? profile.Phone : null;
            ViewState.Address = fields.Address ? profile.Address : null;
            ViewState.City = fields.City ? profile.City : null;
            ViewState.PostCode = fields.PostCode ? profile.PostCode : null;
            ViewState.Country = fields.Country ? profile.Country : null;

            ViewState.PhoneCountries = _profileCompletionSession.PhoneCountries;
            ViewState.DefaultPhoneRegionCode = _profileCompletionSession.DefaultPhoneRegionCode;
            if (ViewState.ShowMobilePhone && !string.IsNullOrEmpty(ViewState.MobilePhone))
                ViewState.PhoneRegionCode = ResolvePhoneRegionCode(ViewState.MobilePhone, ViewState.PhoneCountries);

            ValidatePrefilledProperties();

            ViewState.IsShown = true;
            ViewState.RaiseChanged();
        }

        private void Reset()
        {
            _mobileE164 = null;

            ViewState.FirstName = null;
            ViewState.LastName = null;
            ViewState.BirthDate = null;
            ViewState.Sex = UserSex.Unspecified;
            ViewState.Email = null;
            ViewState.MobilePhone = null;
            ViewState.PhoneRegionCode = null;
            ViewState.Phone = null;
            ViewState.Address = null;
            ViewState.City = null;
            ViewState.PostCode = null;
            ViewState.Country = null;

            ViewState.ShowFirstName = false;
            ViewState.ShowLastName = false;
            ViewState.ShowBirthDate = false;
            ViewState.ShowSex = false;
            ViewState.ShowEmail = false;
            ViewState.ShowMobilePhone = false;
            ViewState.ShowPhone = false;
            ViewState.ShowAddress = false;
            ViewState.ShowCity = false;
            ViewState.ShowPostCode = false;
            ViewState.ShowCountry = false;

            ViewState.PhoneCountries = Array.Empty<Gizmo.Go.Core.Models.PhoneValidation.PhoneCountry>();
            ViewState.DefaultPhoneRegionCode = null;
            ViewState.IsShown = false;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ResetValidationState();

            ViewState.RaiseChanged();
        }

        // only the values the user already has are checked: empty required fields show their message after the first input
        private void ValidatePrefilledProperties()
        {
            if (ViewState.ShowFirstName && !string.IsNullOrEmpty(ViewState.FirstName)) ValidateProperty(() => ViewState.FirstName);
            if (ViewState.ShowLastName && !string.IsNullOrEmpty(ViewState.LastName)) ValidateProperty(() => ViewState.LastName);
            if (ViewState.ShowEmail && !string.IsNullOrEmpty(ViewState.Email)) ValidateProperty(() => ViewState.Email);
            if (ViewState.ShowMobilePhone && !string.IsNullOrEmpty(ViewState.MobilePhone)) ValidateProperty(() => ViewState.MobilePhone);
            if (ViewState.ShowPhone && !string.IsNullOrEmpty(ViewState.Phone)) ValidateProperty(() => ViewState.Phone);
            if (ViewState.ShowAddress && !string.IsNullOrEmpty(ViewState.Address)) ValidateProperty(() => ViewState.Address);
            if (ViewState.ShowCity && !string.IsNullOrEmpty(ViewState.City)) ValidateProperty(() => ViewState.City);
            if (ViewState.ShowPostCode && !string.IsNullOrEmpty(ViewState.PostCode)) ValidateProperty(() => ViewState.PostCode);
            if (ViewState.ShowCountry && !string.IsNullOrEmpty(ViewState.Country)) ValidateProperty(() => ViewState.Country);
        }

        private static string? ResolvePhoneRegionCode(string phoneDigits, IReadOnlyList<Gizmo.Go.Core.Models.PhoneValidation.PhoneCountry> countries)
        {
            var match = countries
                .Where(country => phoneDigits.StartsWith(country.CallingCode.TrimStart('+')))
                .OrderByDescending(country => country.CallingCode.Length)
                .FirstOrDefault();

            // Russia and Kazakhstan share the +7 calling code, Kazakhstan numbers start with 6 or 7 after it
            if (match?.CallingCode.TrimStart('+') == "7" && phoneDigits.Length > 1 && (phoneDigits[1] == '6' || phoneDigits[1] == '7'))
                return countries.FirstOrDefault(country => country.RegionCode == "KZ")?.RegionCode ?? match.RegionCode;

            return match?.RegionCode;
        }

        private static bool IsSamePhone(string e164, string? profilePhone)
        {
            var current = new string(e164.Where(char.IsDigit).ToArray());
            var original = new string((profilePhone ?? string.Empty).Where(char.IsDigit).ToArray()).TrimStart('0');

            // a suffix shorter than a national number would match unrelated numbers
            return original.Length >= MIN_NATIONAL_PHONE_LENGTH && current.EndsWith(original);
        }

        private bool IsEmailChecked => ViewState.ShowEmail
            && !string.IsNullOrEmpty(ViewState.Email)
            && !string.Equals(ViewState.Email, _profileCompletionSession.Profile?.Email, StringComparison.OrdinalIgnoreCase);

        private async Task<bool> ValidateAvailabilityAsync()
        {
            var isAvailable = true;

            if (IsEmailChecked)
            {
                var emailError = await GetEmailAvailabilityErrorAsync(default);
                if (emailError != null)
                {
                    AddError(() => ViewState.Email, emailError);
                    isAvailable = false;
                }
            }

            if (ViewState.ShowMobilePhone && !string.IsNullOrEmpty(_mobileE164))
            {
                var mobileError = await GetMobileAvailabilityErrorAsync(_mobileE164, default);
                if (mobileError != null)
                {
                    AddError(() => ViewState.MobilePhone, mobileError);
                    isAvailable = false;
                }
            }

            return isAvailable;
        }

        private async Task<string?> GetEmailAvailabilityErrorAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (await _registrationService.EmailExistAsync(ViewState.Email!, cancellationToken))
                    return _localizationService.GetString(nameof(Autogenerated.Resources.GO_REGISTRATION_VE_EMAIL_ADDRESS_USED));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.LogError(ex, "Cannot validate email.");
                return _localizationService.GetString(nameof(Autogenerated.Resources.GO_REGISTRATION_VE_CANNOT_VALIDATE_EMAIL));
            }

            return null;
        }

        private async Task<string?> GetMobileAvailabilityErrorAsync(string e164, CancellationToken cancellationToken)
        {
            // the own number of the user is not a conflict, legacy profiles may store it in a non E164 form
            if (IsSamePhone(e164, _profileCompletionSession.Profile?.MobilePhone))
                return null;

            try
            {
                if (await _registrationService.MobilePhoneExistAsync(e164.TrimStart('+'), cancellationToken))
                    return _localizationService.GetString(nameof(Autogenerated.Resources.GO_REGISTRATION_VE_MOBILE_PHONE_USED));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.LogError(ex, "Cannot validate phone.");
                return _localizationService.GetString(nameof(Autogenerated.Resources.GO_REGISTRATION_VE_CANNOT_VALIDATE_PHONE));
            }

            return null;
        }

        private void SetError(string resourceKey)
        {
            ViewState.HasError = true;
            ViewState.ErrorMessage = _localizationService.GetString(resourceKey);
        }

        private void AddRequiredError<T>(System.Linq.Expressions.Expression<Func<T>> accessor)
        {
            AddError(accessor, _localizationService.GetString(nameof(Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.FirstName))
            {
                if (ViewState.ShowFirstName && string.IsNullOrEmpty(ViewState.FirstName))
                    AddRequiredError(() => ViewState.FirstName);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.LastName))
            {
                if (ViewState.ShowLastName && string.IsNullOrEmpty(ViewState.LastName))
                    AddRequiredError(() => ViewState.LastName);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.BirthDate))
            {
                if (ViewState.ShowBirthDate && !ViewState.BirthDate.HasValue)
                    AddRequiredError(() => ViewState.BirthDate);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Sex))
            {
                if (ViewState.ShowSex && ViewState.Sex == UserSex.Unspecified)
                    AddRequiredError(() => ViewState.Sex);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Email))
            {
                if (ViewState.ShowEmail && string.IsNullOrEmpty(ViewState.Email))
                    AddRequiredError(() => ViewState.Email);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.MobilePhone))
            {
                if (ViewState.ShowMobilePhone && string.IsNullOrEmpty(ViewState.MobilePhone))
                    AddRequiredError(() => ViewState.MobilePhone);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Phone))
            {
                if (ViewState.ShowPhone && string.IsNullOrEmpty(ViewState.Phone))
                    AddRequiredError(() => ViewState.Phone);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Address))
            {
                if (ViewState.ShowAddress && string.IsNullOrEmpty(ViewState.Address))
                    AddRequiredError(() => ViewState.Address);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.City))
            {
                if (ViewState.ShowCity && string.IsNullOrEmpty(ViewState.City))
                    AddRequiredError(() => ViewState.City);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.PostCode))
            {
                if (ViewState.ShowPostCode && string.IsNullOrEmpty(ViewState.PostCode))
                    AddRequiredError(() => ViewState.PostCode);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Country))
            {
                if (ViewState.ShowCountry && string.IsNullOrEmpty(ViewState.Country))
                    AddRequiredError(() => ViewState.Country);
            }
        }

        protected override async Task<IEnumerable<string>> OnValidateAsync(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger, CancellationToken cancellationToken = default)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.Email) && IsEmailChecked)
            {
                var emailError = await GetEmailAvailabilityErrorAsync(cancellationToken);
                if (emailError != null)
                    return new string[] { emailError };
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.MobilePhone)
                && ViewState.ShowMobilePhone
                && !string.IsNullOrEmpty(ViewState.MobilePhone))
            {
                if (string.IsNullOrEmpty(ViewState.PhoneRegionCode))
                    return new string[] { _localizationService.GetString(nameof(Autogenerated.Resources.GO_REGISTRATION_VE_SELECT_COUNTRY)) };

                var result = await _phoneValidationService.ValidateAsync(ViewState.MobilePhone, ViewState.PhoneRegionCode, cancellationToken);
                if (!result.IsValid)
                    return new string[] { _localizationService.GetString(result.ErrorKey ?? nameof(Autogenerated.Resources.GO_GEN_AN_ERROR_HAS_OCCURRED)) };

                if (string.IsNullOrWhiteSpace(result.E164))
                    return new string[] { _localizationService.GetString(nameof(Autogenerated.Resources.GO_REGISTRATION_VE_CANNOT_VALIDATE_PHONE)) };

                cancellationToken.ThrowIfCancellationRequested();

                _mobileE164 = result.E164;

                var mobileError = await GetMobileAvailabilityErrorAsync(result.E164, cancellationToken);
                if (mobileError != null)
                    return new string[] { mobileError };
            }

            return await base.OnValidateAsync(fieldIdentifier, validationTrigger, cancellationToken);
        }

        protected override AsyncValidatedDetermineResult OnDetermineIsAsyncPropertiesValidated()
        {
            if (ViewState.ShowMobilePhone
                && !string.IsNullOrEmpty(ViewState.MobilePhone)
                && !IsAsyncValidated(() => ViewState.MobilePhone))
            {
                return base.OnDetermineIsAsyncPropertiesValidated();
            }

            if (IsEmailChecked && !IsAsyncValidated(() => ViewState.Email))
                return base.OnDetermineIsAsyncPropertiesValidated();

            return AsyncValidatedDetermineResult.DefaultTrue;
        }
    }
}
