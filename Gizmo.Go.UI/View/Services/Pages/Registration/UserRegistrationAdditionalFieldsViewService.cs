using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
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
    [Route(NavigationHelper.RegistrationAdditionalFieldsRoute)]
    public sealed class UserRegistrationAdditionalFieldsViewService : ValidatingViewStateServiceBase<UserRegistrationAdditionalFieldsViewState>
    {
        public UserRegistrationAdditionalFieldsViewService(UserRegistrationAdditionalFieldsViewState viewState,
            ILogger<UserRegistrationAdditionalFieldsViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IUserRegistrationService registrationService,
            IRegistrationSessionService registrationSession) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _registrationService = registrationService;
            _registrationSession = registrationSession;
        }

        private readonly ILocalizationService _localizationService;
        private readonly IUserRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;

        private bool ShowCountry => _registrationSession.RequiredUserInfo?.Country == true;
        private bool ShowAddress => _registrationSession.RequiredUserInfo?.Address == true;
        private bool ShowCity => _registrationSession.RequiredUserInfo?.City == true;
        private bool ShowPostCode => _registrationSession.RequiredUserInfo?.PostCode == true;

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

        public void SetMobilePhone(string? value)
        {
            ViewState.MobilePhone = value;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void Clear()
        {
            ViewState.Address = null;
            ViewState.City = null;
            ViewState.PostCode = null;
            ViewState.Country = null;
            //ViewState.Prefix = null;
            ViewState.MobilePhone = null;

            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ResetValidationState();
            DebounceViewStateChanged();
        }

        private void OnRegistrationSessionCleared(object? sender, EventArgs e) => Clear();

        public async Task SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            if (!await ValidateProfileEmailAvailabilityAsync())
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
                return;
            }

            bool confirmationRequired = !string.IsNullOrEmpty(_registrationSession.Token);

            try
            {
                string? mobilePhone = _registrationSession.HasConfirmedMobilePhone
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
                    Address = ShowAddress ? ViewState.Address : null,
                    City = ShowCity ? ViewState.City : null,
                    PostCode = ShowPostCode ? ViewState.PostCode : null,
                    Country = ShowCountry ? ViewState.Country : null,
                    MobilePhone = mobilePhone,
                    Phone = _registrationSession.Phone
                };

                RegistrationCompleteCode result;
                if (confirmationRequired)
                {
                    result = await _registrationService.CompleteAsync(new RegistrationCompleteRequest
                    {
                        Token = _registrationSession.Token,
                        Profile = profile,
                        Password = _registrationSession.Password,
                        AgreementStates = _registrationSession.AgreementChoices
                    });
                }
                else
                {
                    result = await _registrationService.DirectAsync(new RegistrationCompleteRequest
                    {
                        Profile = profile,
                        Password = _registrationSession.Password,
                        AgreementStates = _registrationSession.AgreementChoices
                    });
                }

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

        private async Task<bool> ValidateProfileEmailAvailabilityAsync()
        {
            if (_registrationSession.Flow == RegistrationFlow.Email || string.IsNullOrWhiteSpace(_registrationSession.Email))
                return true;

            try
            {
                if (!await _registrationService.EmailExistAsync(_registrationSession.Email))
                    return true;

                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_VE_EMAIL_ADDRESS_USED));
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot validate email.");

                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_REGISTRATION_VE_CANNOT_VALIDATE_EMAIL));
                return false;
            }
        }

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
        }

        public Task NavigateBackAsync()
        {
            NavigationService.NavigateTo(NavigationHelper.RegistrationBasicFieldsProfileRoute);
            return Task.CompletedTask;
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _registrationSession.Cleared += OnRegistrationSessionCleared;

            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _registrationSession.Cleared -= OnRegistrationSessionCleared;

            base.OnDisposing(isDisposing);
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.Country))
            {
                if (ShowCountry && string.IsNullOrEmpty(ViewState.Country))
                {
                    AddError(() => ViewState.Country, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Address))
            {
                if (ShowAddress && string.IsNullOrEmpty(ViewState.Address))
                {
                    AddError(() => ViewState.Address, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.City))
            {
                if (ShowCity && string.IsNullOrEmpty(ViewState.City))
                {
                    AddError(() => ViewState.City, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.PostCode))
            {
                if (ShowPostCode && string.IsNullOrEmpty(ViewState.PostCode))
                {
                    AddError(() => ViewState.PostCode, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_VE_REQUIRED_FIELD)));
                }
            }
        }
    }
}
