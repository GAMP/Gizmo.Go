using System.Text.RegularExpressions;
using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Common;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.Go.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.UI;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationBasicFieldsRoute)]
    [Route(NavigationHelper.RegistrationBasicFieldsProfileRoute)]
    public sealed class UserRegistrationBasicFieldsViewService : ValidatingViewStateServiceBase<UserRegistrationBasicFieldsViewState>
    {
        public UserRegistrationBasicFieldsViewService(UserRegistrationBasicFieldsViewState viewState,
            ILogger<UserRegistrationBasicFieldsViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IUserRegistrationService registrationService,
            IMobileOptionsService mobileOptionsService,
            IRegistrationSessionService registrationSession,
            IPhoneValidationService phoneValidationService,
            IAuthOptionsService authOptionsService) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _registrationService = registrationService;
            _mobileOptionsService = mobileOptionsService;
            _registrationSession = registrationSession;
            _phoneValidationService = phoneValidationService;
            _authOptionsService = authOptionsService;
        }

        private readonly ILocalizationService _localizationService;
        private readonly IUserRegistrationService _registrationService;
        private readonly IMobileOptionsService _mobileOptionsService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly IPhoneValidationService _phoneValidationService;
        private readonly IAuthOptionsService _authOptionsService;
        private bool _passwordRulesLoaded;

        private bool HasAdditionalFields =>
            _registrationSession.RequiredUserInfo?.Country == true ||
            _registrationSession.RequiredUserInfo?.Address == true ||
            _registrationSession.RequiredUserInfo?.City == true ||
            _registrationSession.RequiredUserInfo?.PostCode == true;

        private bool ShowPassword => true;
        private bool ShowFirstName => _registrationSession.RequiredUserInfo?.FirstName == true;
        private bool ShowLastName => _registrationSession.RequiredUserInfo?.LastName == true;
        private bool ShowBirthDate => _registrationSession.RequiredUserInfo?.BirthDate == true;
        private bool ShowSex => _registrationSession.RequiredUserInfo?.Sex == true;
        private bool ShowEmail => _registrationSession.Flow != RegistrationFlow.Email && _registrationSession.RequiredUserInfo?.Email == true;
        private bool ShowMobilePhone => !_registrationSession.HasConfirmedMobilePhone && _registrationSession.RequiredUserInfo?.Mobile == true;
        private bool ShowPhone => _registrationSession.RequiredUserInfo?.Phone == true;

        public void SetUsername(string value)
        {
            ViewState.Username = value;
            ValidateProperty(() => ViewState.Username);
        }

        public void SetPassword(string value)
        {
            ViewState.Password = value;
            CheckPasswordRules(ViewState.Password);
            ValidateProperty(() => ViewState.Password);
            ValidateProperty(() => ViewState.RepeatPassword);
        }

        public void SetRepeatPassword(string value)
        {
            ViewState.RepeatPassword = value;
            ValidateProperty(() => ViewState.RepeatPassword);
        }

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
            _registrationSession.SetPhoneE164(null);
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void SetPhone(string? value)
        {
            ViewState.Phone = value;
            ValidateProperty(() => ViewState.Phone);
        }

        public void SetPhoneRegionCode(string? value)
        {
            ViewState.PhoneRegionCode = value;
            _registrationSession.SetPhoneE164(null);
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void Clear()
        {
            ViewState.Username = string.Empty;
            ViewState.Password = string.Empty;
            ViewState.RepeatPassword = string.Empty;
            ViewState.FirstName = null;
            ViewState.LastName = null;
            ViewState.BirthDate = null;
            ViewState.Sex = UserSex.Unspecified;
            ViewState.Email = null;
            ViewState.MobilePhone = null;
            ViewState.Phone = null;
            ViewState.PhoneRegionCode = null;

            ViewState.IsAccountStepCompleted = false;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            CheckPasswordRules(ViewState.Password);
            ResetValidationState();
            DebounceViewStateChanged();
        }

        private void OnRegistrationSessionCleared(object? sender, EventArgs e) => Clear();

        /// <summary>
        /// Validates the account step (password) and opens the profile step.
        /// </summary>
        public Task ContinueToProfileAsync()
        {
            // the username is validated while typing: only an empty one needs the required message here,
            // restarting its async validation would disable the submit button before the click is handled
            if (string.IsNullOrEmpty(ViewState.Username))
                ValidateProperty(() => ViewState.Username);

            ValidateProperty(() => ViewState.Password);
            ValidateProperty(() => ViewState.RepeatPassword);

            if (EditContext.GetValidationMessages(FieldIdentifier.Create(() => ViewState.Username)).Any()
                || !IsAsyncValidated(() => ViewState.Username))
            {
                return Task.CompletedTask;
            }

            if (EditContext.GetValidationMessages(FieldIdentifier.Create(() => ViewState.Password)).Any()
                || EditContext.GetValidationMessages(FieldIdentifier.Create(() => ViewState.RepeatPassword)).Any())
            {
                return Task.CompletedTask;
            }

            ViewState.IsAccountStepCompleted = true;
            ViewState.RaiseChanged();

            NavigationService.NavigateTo(NavigationHelper.RegistrationBasicFieldsProfileRoute);
            return Task.CompletedTask;
        }

        public Task NavigateBackAsync()
        {
            // the session is cleared (agreements, required info), so a new attempt starts from the Index step
            var route = _registrationSession.Flow == RegistrationFlow.None
                ? NavigationHelper.WelcomePage
                : NavigationHelper.RegistrationIndexRoute;

            _registrationSession.Clear();
            NavigationService.NavigateTo(route);
            return Task.CompletedTask;
        }

        public Task NavigateBackToAccountAsync()
        {
            NavigationService.NavigateTo(NavigationHelper.RegistrationBasicFieldsRoute);
            return Task.CompletedTask;
        }

        private void UpdateFieldVisibility()
        {
            ViewState.ShowFirstName = ShowFirstName;
            ViewState.ShowLastName = ShowLastName;
            ViewState.ShowBirthDate = ShowBirthDate;
            ViewState.ShowSex = ShowSex;
            ViewState.ShowEmail = ShowEmail;
            ViewState.ShowMobilePhone = ShowMobilePhone;
            ViewState.ShowPhone = ShowPhone;
            ViewState.HasAdditionalFields = HasAdditionalFields;

            ViewState.IsEmailVerified = _registrationSession.Flow == RegistrationFlow.Email;

            ViewState.RaiseChanged();
        }

        private async Task UpdateVerifiedContactAsync(CancellationToken cancellationToken)
        {
            ViewState.VerifiedContact = string.IsNullOrEmpty(_registrationSession.ActualContact)
                ? string.Empty
                : ViewState.IsEmailVerified
                    ? ContactMasking.MaskEmail(_registrationSession.ActualContact)
                    : ContactMasking.MaskPhone(_registrationSession.ActualContact, await _phoneValidationService.GetCountriesAsync(cancellationToken));

            ViewState.RaiseChanged();
        }

        private string GetCurrentLocation() =>
            new Uri(NavigationService.GetUri()).LocalPath.TrimEnd('/');

        public async Task SubmitAsync()
        {
            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            Validate();

            if (ViewState.IsValid != true)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();

                return;
            }

            if (!await ValidateProfileEmailAvailabilityAsync())
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
                return;
            }

            var sessionEmail = _registrationSession.Flow == RegistrationFlow.Email
                ? _registrationSession.ActualContact
                : ShowEmail ? ViewState.Email : null;

            _registrationSession.SetProfileBasics(
                ViewState.Username,
                ViewState.Password,
                ShowFirstName ? ViewState.FirstName : null,
                ShowLastName ? ViewState.LastName : null,
                ShowBirthDate ? ViewState.BirthDate : null,
                ShowSex ? ViewState.Sex : UserSex.Unspecified,
                sessionEmail);

            var mobilePhone = ShowMobilePhone && !string.IsNullOrEmpty(ViewState.MobilePhone)
                ? _registrationSession.PhoneE164
                : null;

            _registrationSession.SetMobilePhone(mobilePhone);
            _registrationSession.SetPhone(ShowPhone ? ViewState.Phone : null);

            if (!HasAdditionalFields)
            {
                try
                {
                    string? profileMobilePhone = _registrationSession.HasConfirmedMobilePhone
                        ? _registrationSession.ActualContact
                        : _registrationSession.MobilePhone;

                    var profile = new RegistrationProfile
                    {
                        Username = _registrationSession.Username,
                        FirstName = _registrationSession.FirstName,
                        LastName = _registrationSession.LastName,
                        BirthDate = _registrationSession.BirthDate,
                        Sex = _registrationSession.Sex,
                        Email = _registrationSession.Flow == RegistrationFlow.Email
                            ? _registrationSession.ActualContact
                            : _registrationSession.Email,
                        MobilePhone = profileMobilePhone,
                        Phone = _registrationSession.Phone
                    };

                    // mobile deviation: direct registration is not supported, the verification token is always present
                    var result = await _registrationService.CompleteAsync(new RegistrationCompleteRequest
                    {
                        Token = _registrationSession.Token,
                        Profile = profile,
                        Password = _registrationSession.Password,
                        AgreementStates = _registrationSession.AgreementChoices
                    });

                    if (result != RegistrationCompleteCode.Success)
                    {
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_FAILED_MESSAGE));
                        return;
                    }

                    _registrationSession.SetCompletedContact(profile.MobilePhone ?? profile.Email, isEmail: string.IsNullOrEmpty(profile.MobilePhone));
                    _registrationSession.Clear();
                    NavigationService.NavigateTo(NavigationHelper.RegistrationSuccessRoute);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "User create complete error.");

                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_AN_ERROR_HAS_OCCURRED));
                }
                finally
                {
                    ViewState.IsLoading = false;
                    ViewState.RaiseChanged();
                }
            }
            else
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();

                NavigationService.NavigateTo(NavigationHelper.RegistrationAdditionalFieldsRoute);
            }
        }

        private async Task<bool> ValidateProfileEmailAvailabilityAsync()
        {
            if (!ShowEmail || _registrationSession.Flow == RegistrationFlow.Email || string.IsNullOrWhiteSpace(ViewState.Email))
                return true;

            try
            {
                if (!await _registrationService.EmailExistAsync(ViewState.Email))
                    return true;

                AddError(() => ViewState.Email, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_VE_EMAIL_ADDRESS_USED)));
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot validate email.");
                AddError(() => ViewState.Email, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_VE_CANNOT_VALIDATE_EMAIL)));
                return false;
            }
        }

        private void CheckPasswordRules(string password)
        {
            ViewState.PasswordTooltip.PassedRules = 0;
            ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_PASSWORD_MESSAGE_TOO_SHORT));

            ViewState.PasswordTooltip.LengthRulePassed = false;
            ViewState.PasswordTooltip.LowerCaseCharactersRulePassed = false;
            ViewState.PasswordTooltip.UpperCaseCharactersRulePassed = false;
            ViewState.PasswordTooltip.NumbersRulePassed = false;

            if (string.IsNullOrEmpty(password))
                return;

            if (ViewState.PasswordTooltip.MinimumLengthRule > 0 || ViewState.PasswordTooltip.MaximumLengthRule > 0)
            {
                if ((!ViewState.PasswordTooltip.MinimumLengthRule.HasValue || password.Length >= ViewState.PasswordTooltip.MinimumLengthRule) &&
                    (!ViewState.PasswordTooltip.MaximumLengthRule.HasValue || password.Length <= ViewState.PasswordTooltip.MaximumLengthRule))
                {
                    ViewState.PasswordTooltip.PassedRules += 1;
                    ViewState.PasswordTooltip.LengthRulePassed = true;
                }
                else
                {
                    if (password.Length >= ViewState.PasswordTooltip.MaximumLengthRule)
                    {
                        ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_PASSWORD_MESSAGE_TOO_LONG));
                    }
                    else
                    {
                        ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_PASSWORD_MESSAGE_TOO_SHORT));
                    }
                }
            }

            Regex lowerRulePassedRegex = new Regex("\\p{Ll}");
            Regex upperRuleRegex = new Regex("\\p{Lu}");
            Regex numberRuleRegex = new Regex("[0-9]");

            if (ViewState.PasswordTooltip.HasLowerCaseCharactersRule)
            {
                if (lowerRulePassedRegex.Matches(password).Count > 0)
                {
                    ViewState.PasswordTooltip.PassedRules += 1;
                    ViewState.PasswordTooltip.LowerCaseCharactersRulePassed = true;
                }
                else
                {
                    ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_PASSWORD_MESSAGE_TOO_EASY));
                }
            }

            if (ViewState.PasswordTooltip.HasUpperCaseCharactersRule)
            {
                if (upperRuleRegex.Matches(password).Count > 0)
                {
                    ViewState.PasswordTooltip.PassedRules += 1;
                    ViewState.PasswordTooltip.UpperCaseCharactersRulePassed = true;
                }
                else
                {
                    ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_PASSWORD_MESSAGE_TOO_EASY));
                }
            }

            if (ViewState.PasswordTooltip.HasNumbersRule)
            {
                if (numberRuleRegex.Matches(password).Count > 0)
                {
                    ViewState.PasswordTooltip.PassedRules += 1;
                    ViewState.PasswordTooltip.NumbersRulePassed = true;
                }
                else
                {
                    ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_PASSWORD_MESSAGE_TOO_EASY));
                }
            }

            if (ViewState.PasswordTooltip.PassedRules == ViewState.PasswordTooltip.TotalRules)
            {
                ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_PASSWORD_MESSAGE_SECURE));
            }
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            await TryEnsurePasswordRulesAsync(cancellationToken);

            // the service is associated with two routes: OnNavigatedIn fires on the password -> profile transition too
            var isProfileRoute = GetCurrentLocation().EndsWith(NavigationHelper.RegistrationBasicFieldsProfileRoute, StringComparison.OrdinalIgnoreCase);

            if (isProfileRoute)
            {
                if (!ViewState.IsAccountStepCompleted)
                {
                    NavigationService.NavigateTo(NavigationHelper.RegistrationBasicFieldsRoute);
                    return;
                }

                UpdateFieldVisibility();

                if (ShowMobilePhone)
                {
                    ViewState.DefaultPhoneRegionCode = (await _authOptionsService.GetAsync(cancellationToken))?.CountryCode;
                    ViewState.PhoneCountries = await _phoneValidationService.GetCountriesAsync(cancellationToken);

                    if (ViewState.PhoneCountries.Count == 0)
                    {
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_AN_ERROR_HAS_OCCURRED));
                    }

                    ViewState.RaiseChanged();
                }

                return;
            }

            if (ViewState.IsAccountStepCompleted)
            {
                // back from the profile step: keep the entered values, skip the entry checks
                ViewState.IsAccountStepCompleted = false;
                ViewState.RaiseChanged();
                return;
            }

            if (string.IsNullOrEmpty(_registrationSession.Token) && _registrationSession.Flow != RegistrationFlow.None)
            {
                NavigationService.NavigateTo(NavigationHelper.RegistrationProvidersRoute);
                return;
            }

            UpdateFieldVisibility();
            await UpdateVerifiedContactAsync(cancellationToken);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _registrationSession.Cleared -= OnRegistrationSessionCleared;

            base.OnDisposing(isDisposing);
        }

        // mobile deviation: the password rules come from the server mobile options, loaded on the page entry
        // (not in OnInitializing: the view services are initialized one by one on start, an unreachable server would block it)
        private async Task EnsurePasswordRulesAsync(CancellationToken ct)
        {
            if (_passwordRulesLoaded)
                return;

            var passwordValidation = (await _mobileOptionsService.GetAsync(ct)).PasswordValidation;

            ViewState.PasswordTooltip.MinimumLengthRule = passwordValidation.MinimumLength;
            ViewState.PasswordTooltip.MaximumLengthRule = passwordValidation.MaximumLength;
            ViewState.PasswordTooltip.HasLowerCaseCharactersRule = passwordValidation.LowerCaseCharactersRequired;
            ViewState.PasswordTooltip.HasUpperCaseCharactersRule = passwordValidation.UpperCaseCharactersRequired;
            ViewState.PasswordTooltip.HasNumbersRule = passwordValidation.NumbersRequired;

            ViewState.PasswordTooltip.TotalRules = 0;

            if (ViewState.PasswordTooltip.MinimumLengthRule > 0 || ViewState.PasswordTooltip.MaximumLengthRule > 0)
                ViewState.PasswordTooltip.TotalRules += 1;

            if (ViewState.PasswordTooltip.HasLowerCaseCharactersRule)
                ViewState.PasswordTooltip.TotalRules += 1;

            if (ViewState.PasswordTooltip.HasUpperCaseCharactersRule)
                ViewState.PasswordTooltip.TotalRules += 1;

            if (ViewState.PasswordTooltip.HasNumbersRule)
                ViewState.PasswordTooltip.TotalRules += 1;

            CheckPasswordRules(ViewState.Password);

            ViewState.PasswordTooltip.RaiseChanged();

            _passwordRulesLoaded = true;
        }

        private async Task TryEnsurePasswordRulesAsync(CancellationToken ct)
        {
            try
            {
                await EnsurePasswordRulesAsync(ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.LogError(ex, "Could not load the password rules.");
            }
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _registrationSession.Cleared += OnRegistrationSessionCleared;

            return base.OnInitializing(ct);
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.Username))
            {
                if (!string.IsNullOrEmpty(ViewState.Username) && ViewState.Username.ToString().Any(c => char.IsWhiteSpace(c)))
                {
                    ClearError(() => ViewState.Username);
                    AddError(() => ViewState.Username, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_VE_WHITE_SPACE_NOT_ALLOWED)));
                }
            }
            else if (fieldIdentifier.FieldEquals(() => ViewState.Password) || fieldIdentifier.FieldEquals(() => ViewState.RepeatPassword))
            {
                if (ShowPassword)
                {
                    if (fieldIdentifier.FieldEquals(() => ViewState.Password))
                    {
                        if (string.IsNullOrEmpty(ViewState.Password))
                        {
                            AddError(() => ViewState.Password, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
                        }
                        else if (ViewState.PasswordTooltip.PassedRules < ViewState.PasswordTooltip.TotalRules)
                        {
                            AddError(() => ViewState.Password, ViewState.PasswordTooltip.ErrorMessage);
                        }
                    }

                    if (fieldIdentifier.FieldEquals(() => ViewState.RepeatPassword))
                    {
                        if (string.IsNullOrEmpty(ViewState.RepeatPassword))
                        {
                            AddError(() => ViewState.RepeatPassword, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
                        }
                        else if (!string.IsNullOrEmpty(ViewState.Password) && string.Compare(ViewState.Password, ViewState.RepeatPassword) != 0)
                        {
                            AddError(() => ViewState.RepeatPassword, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_PASSWORDS_DO_NOT_MATCH)));
                        }
                    }
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.FirstName))
            {
                if (ShowFirstName && string.IsNullOrEmpty(ViewState.FirstName))
                {
                    AddError(() => ViewState.FirstName, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.LastName))
            {
                if (ShowLastName && string.IsNullOrEmpty(ViewState.LastName))
                {
                    AddError(() => ViewState.LastName, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.BirthDate))
            {
                if (ShowBirthDate && !ViewState.BirthDate.HasValue)
                {
                    AddError(() => ViewState.BirthDate, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Sex))
            {
                if (ShowSex && ViewState.Sex == UserSex.Unspecified)
                {
                    AddError(() => ViewState.Sex, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Email))
            {
                if (ShowEmail)
                {
                    if (string.IsNullOrEmpty(ViewState.Email))
                    {
                        AddError(() => ViewState.Email, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
                    }
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.MobilePhone))
            {
                if (ShowMobilePhone && string.IsNullOrEmpty(ViewState.MobilePhone))
                {
                    AddError(() => ViewState.MobilePhone, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Phone))
            {
                if (ShowPhone && string.IsNullOrEmpty(ViewState.Phone))
                {
                    AddError(() => ViewState.Phone, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
                }
            }
        }

        protected override async Task<IEnumerable<string>> OnValidateAsync(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger, CancellationToken cancellationToken = default)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.Username))
            {
                if (!string.IsNullOrEmpty(ViewState.Username))
                {
                    try
                    {
                        if (await _registrationService.UserNameExistAsync(ViewState.Username, cancellationToken))
                        {
                            return new string[] { _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_VE_USERNAME_IN_USE)) };
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Cannot validate username.");
                        return new string[] { _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_VE_CANNOT_VALIDATE_USERNAME)) };
                    }
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Email)
                && ShowEmail
                && !string.IsNullOrEmpty(ViewState.Email))
            {
                try
                {
                    if (await _registrationService.EmailExistAsync(ViewState.Email, cancellationToken))
                    {
                        return new string[] { _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_VE_EMAIL_ADDRESS_USED)) };
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Cannot validate email.");
                    return new string[] { _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_VE_CANNOT_VALIDATE_EMAIL)) };
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.MobilePhone)
                && ShowMobilePhone
                && !string.IsNullOrEmpty(ViewState.MobilePhone)
                )
            {
                if (string.IsNullOrEmpty(ViewState.PhoneRegionCode))
                    return new string[] { _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_VE_SELECT_COUNTRY)) };

                var result = await _phoneValidationService.ValidateAsync(ViewState.MobilePhone, ViewState.PhoneRegionCode, cancellationToken);
                if (!result.IsValid)
                    return new string[] { _localizationService.GetString(result.ErrorKey ?? nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_AN_ERROR_HAS_OCCURRED)) };

                if (string.IsNullOrWhiteSpace(result.E164))
                    return new string[] { _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_VE_CANNOT_VALIDATE_PHONE)) };

                _registrationSession.SetPhoneE164(result.E164);

                try
                {
                    var phone = result.E164;
                    if (phone.StartsWith("+"))
                        phone = phone.Substring(1);

                    if (await _registrationService.MobilePhoneExistAsync(phone, cancellationToken))
                    {
                        return new string[] { _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_VE_MOBILE_PHONE_USED)) };
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Cannot validate phone.");
                    return new string[] { _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_VE_CANNOT_VALIDATE_PHONE)) };
                }
            }

            return await base.OnValidateAsync(fieldIdentifier, validationTrigger, cancellationToken);
        }

        protected override void OnValidationStateChanged()
        {
            ViewState.IsUsernameAvailable = !string.IsNullOrEmpty(ViewState.Username)
                && IsAsyncValidated(() => ViewState.Username)
                && !EditContext.GetValidationMessages(FieldIdentifier.Create(() => ViewState.Username)).Any();

            base.OnValidationStateChanged();
        }

        protected override AsyncValidatedDetermineResult OnDetermineIsAsyncPropertiesValidated()
        {
            // Username is required: always check it first
            if (!IsAsyncValidated(() => ViewState.Username))
                return base.OnDetermineIsAsyncPropertiesValidated();

            if (ShowMobilePhone
                && !string.IsNullOrEmpty(ViewState.MobilePhone)
                && !IsAsyncValidated(() => ViewState.MobilePhone))
            {
                return base.OnDetermineIsAsyncPropertiesValidated();
            }

            if (ShowEmail
                && !string.IsNullOrEmpty(ViewState.Email)
                && !IsAsyncValidated(() => ViewState.Email))
            {
                return base.OnDetermineIsAsyncPropertiesValidated();
            }

            return AsyncValidatedDetermineResult.DefaultTrue;
        }

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
        }
    }
}
