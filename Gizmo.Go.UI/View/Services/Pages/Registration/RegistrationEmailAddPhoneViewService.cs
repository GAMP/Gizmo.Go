using System.Globalization;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.Models;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationEmailAddPhone)]
    public sealed class RegistrationEmailAddPhoneViewService : ValidatingViewStateServiceBase<RegistrationEmailAddPhoneViewState>
    {
        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;
        private readonly IPhoneValidationService _phoneValidationService;
        private readonly IRegistrationSessionService _registrationSession;

        public RegistrationEmailAddPhoneViewService(
            RegistrationEmailAddPhoneViewState viewState,
            ILogger<RegistrationEmailAddPhoneViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IPhoneValidationService phoneValidationService,
            IRegistrationSessionService registrationSession) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _phoneValidationService = phoneValidationService;
            _registrationSession = registrationSession;
        }

        #endregion

        #region METHODS

        public ValueTask UpdatePhoneAsync(string phoneInput, string countryIso2)
        {
            var country = PhoneCountryList.FindByIso2(countryIso2);

            var result = !string.IsNullOrWhiteSpace(phoneInput) && country is not null
                ? _phoneValidationService.Validate(phoneInput, country.Iso2)
                : PhoneValidationResult.Invalid;

            var maxDigits = country is not null
                ? _phoneValidationService.GetMaxLength(country.Iso2)
                : 15;

            const int separatorBuffer = 6;

            ViewState.Phone = phoneInput;
            ViewState.PhoneInput = phoneInput;
            ViewState.PhoneE164 = result.E164;
            ViewState.SelectedCountryIso2 = country?.Iso2 ?? string.Empty;
            ViewState.SelectedCountryName = country?.Name ?? string.Empty;
            ViewState.SelectedDialCode = country?.DialCode ?? string.Empty;
            ViewState.SelectedCountryPlaceholder = country?.Placeholder ?? string.Empty;
            ViewState.PhoneLength = maxDigits + separatorBuffer;

            ValidateProperty(() => ViewState.PhoneInput);
            ViewState.RaiseChanged();

            return ValueTask.CompletedTask;
        }

        public ValueTask SubmitAsync()
        {
            if (!ViewState.CanSubmit || ViewState.IsSubmitting)
                return ValueTask.CompletedTask;

            ViewState.IsSubmitting = true;
            ViewState.RaiseChanged();

            _registrationSession.SetPhone(ViewState.PhoneE164);
            _navigationService.NavigateTo(NavigationHelper.RegistrationPasswordPage);
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateBackAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationEmailConfirmation);
            return ValueTask.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (!_registrationSession.HasToken)
            {
                _navigationService.NavigateTo(NavigationHelper.RegistrationEmail);
                return base.OnNavigatedIn(navigationParameters, cancellationToken);
            }

            const int separatorBuffer = 6;
            var culture = CultureInfo.CurrentUICulture.Name;
            var parts = culture.Split('-');
            var defaultIso2 = parts.Length > 1 ? parts[^1] : "US";
            var defaultCountry = PhoneCountryList.FindByIso2(defaultIso2);

            ViewState.ConfirmedEmail = _registrationSession.Email;
            ViewState.Phone = string.Empty;
            ViewState.PhoneInput = string.Empty;
            ViewState.PhoneE164 = string.Empty;
            ViewState.SelectedCountryIso2 = defaultCountry?.Iso2 ?? string.Empty;
            ViewState.SelectedCountryName = defaultCountry?.Name ?? string.Empty;
            ViewState.SelectedDialCode = defaultCountry?.DialCode ?? string.Empty;
            ViewState.SelectedCountryPlaceholder = defaultCountry?.Placeholder ?? string.Empty;
            ViewState.PhoneLength = _phoneValidationService.GetMaxLength(defaultIso2) + separatorBuffer;
            ViewState.IsSubmitting = false;
            ViewState.ErrorMessage = null;

            ClearError(() => ViewState.PhoneInput);

            ViewState.RaiseChanged();

            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger trigger)
        {
            if (fieldIdentifier.FieldName
                is nameof(RegistrationEmailAddPhoneViewState.PhoneInput)
                or nameof(RegistrationEmailAddPhoneViewState.SelectedCountryIso2))
            {
                ClearError(() => ViewState.PhoneInput);

                if (string.IsNullOrWhiteSpace(ViewState.SelectedCountryIso2))
                {
                    AddError(() => ViewState.PhoneInput, Autogenerated.Resources.GO_VE_PHONE_COUNTRY_REQUIRED);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ViewState.PhoneInput))
                {
                    AddError(() => ViewState.PhoneInput, Autogenerated.Resources.GO_VE_PHONE_REQUIRED);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ViewState.PhoneE164))
                    AddError(() => ViewState.PhoneInput, Autogenerated.Resources.GO_VE_PHONE_INVALID);
            }
        }

        #endregion
    }
}
