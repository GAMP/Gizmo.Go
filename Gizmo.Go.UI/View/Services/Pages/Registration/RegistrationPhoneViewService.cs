using System.Globalization;
using System.Web;
using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
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
    [Route(NavigationHelper.RegistrationPhone)]
    public sealed class RegistrationPhoneViewService : ValidatingViewStateServiceBase<RegistrationPhoneViewState>
    {
        private readonly NavigationService _navigationService;
        private readonly IRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly IPhoneValidationService _phoneValidationService;
        private readonly ILocalizationService _localizationService;

        public RegistrationPhoneViewService(
            RegistrationPhoneViewState viewState,
            ILogger<RegistrationPhoneViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IRegistrationService registrationService,
            IRegistrationSessionService registrationSession,
            IPhoneValidationService phoneValidationService,
            ILocalizationService localizationService)
            : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _registrationService = registrationService;
            _registrationSession = registrationSession;
            _phoneValidationService = phoneValidationService;
            _localizationService = localizationService;
        }

        public ValueTask UpdatePhoneAsync(string phoneInput, string countryIso2)
        {
            var country = PhoneCountryList.FindByIso2(countryIso2);

            var result = !string.IsNullOrWhiteSpace(phoneInput) && country is not null
                ? _phoneValidationService.Validate(phoneInput, country.Iso2)
                : PhoneValidationResult.Invalid;

            var maxDigits = country is not null
                ? _phoneValidationService.GetMaxLength(country.Iso2)
                : 15;

            const int separatorBuffer = 6; //TODO временное решение

            ViewState.PhoneInput = phoneInput;
            ViewState.PhoneE164 = result.E164;
            ViewState.FormattedPhoneInput = result.FormattedNational;
            ViewState.SelectedCountryIso2 = country?.Iso2 ?? string.Empty;
            ViewState.SelectedCountryName = country?.Name ?? string.Empty;
            ViewState.SelectedDialCode = country?.DialCode ?? string.Empty;
            ViewState.SelectedCountryPlaceholder = country?.Placeholder ?? string.Empty;
            ViewState.PhoneLength = maxDigits + separatorBuffer;

            ValidateProperty(() => ViewState.PhoneInput);
            ViewState.RaiseChanged();

            return ValueTask.CompletedTask;
        }

        public ValueTask ToggleTermsAsync()
        {
            ViewState.TermsAccepted = !ViewState.TermsAccepted;
            ValidateProperty(() => ViewState.TermsAccepted);
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public async Task SubmitAsync(CancellationToken cancellationToken = default)
        {
            Validate();

            if (ViewState.IsValid != true)
            {
                ViewState.RaiseChanged();
                return;
            }

            var request = new RegistrationStartRequest
            {
                IntegrationPublicId = ViewState.IntegrationPublicId,
                DeliveryMethod = RegistrationDeliveryMethod.CodeDispatch,
                PhoneNumber = ViewState.PhoneE164
            };

            RegistrationStartResult result;
            try
            {
                result = await _registrationService.StartAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to start SMS registration for provider {IntegrationPublicId}.", ViewState.IntegrationPublicId);
                _navigationService.NavigateTo(NavigationHelper.RegistrationProviders + "?error=1");
                return;
            }

            if (result.Result != RegistrationStartResultCode.Success || string.IsNullOrEmpty(result.Token))
            {
                Logger.LogWarning("Registration start failed with result {Result}.", result.Result);
                _navigationService.NavigateTo(NavigationHelper.RegistrationProviders + "?error=1");
                return;
            }

            _registrationSession.SetToken(result.Token);
            _registrationSession.SetCodeLength(result.CodeLength);
            _registrationSession.SetPhone(ViewState.PhoneE164);
            _registrationSession.SetFlow(RegistrationFlow.Sms);
            _navigationService.NavigateTo(NavigationHelper.RegistrationConfirmationPage);
        }

        public ValueTask NavigateBackAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
            return ValueTask.CompletedTask;
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            var uri = new Uri(_navigationService.GetUri());
            var raw = HttpUtility.ParseQueryString(uri.Query).Get("provider");

            if (!Guid.TryParse(raw, out var channelId))
            {
                _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
                //TODO кинуть уведомление о том что провадер не найден
                return;
            }

            ViewState.IntegrationPublicId = channelId;

            const int separatorBuffer = 6; //TODO временное решение
            var culture = CultureInfo.CurrentUICulture.Name;
            var parts = culture.Split('-');
            var defaultIso2 = parts.Length > 1 ? parts[^1] : "US";
            var defaultCountry = PhoneCountryList.FindByIso2(defaultIso2);

            ViewState.PhoneInput = string.Empty;
            ViewState.PhoneE164 = string.Empty;
            ViewState.FormattedPhoneInput = string.Empty;
            ViewState.SelectedCountryIso2 = defaultCountry?.Iso2 ?? string.Empty;
            ViewState.SelectedCountryName = defaultCountry?.Name ?? string.Empty;
            ViewState.SelectedDialCode = defaultCountry?.DialCode ?? string.Empty;
            ViewState.SelectedCountryPlaceholder = defaultCountry?.Placeholder ?? string.Empty;
            ViewState.PhoneLength = _phoneValidationService.GetMaxLength(defaultIso2) + separatorBuffer;
            ViewState.TermsAccepted = false;

            ClearError(() => ViewState.PhoneInput);
            ClearError(() => ViewState.TermsAccepted);

            ViewState.RaiseChanged();

            await base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger trigger)
        {
            if (fieldIdentifier.FieldName
                is nameof(RegistrationPhoneViewState.PhoneInput)
                or nameof(RegistrationPhoneViewState.SelectedCountryIso2))
            {
                ClearError(() => ViewState.PhoneInput);

                if (string.IsNullOrWhiteSpace(ViewState.SelectedCountryIso2))
                {
                    AddError(() => ViewState.PhoneInput, _localizationService.GetString(nameof(Autogenerated.Resources.GO_VE_PHONE_COUNTRY_REQUIRED)));
                    return;
                }

                if (string.IsNullOrWhiteSpace(ViewState.PhoneInput))
                {
                    AddError(() => ViewState.PhoneInput, _localizationService.GetString(nameof(Autogenerated.Resources.GO_VE_PHONE_REQUIRED)));
                    return;
                }

                if (string.IsNullOrWhiteSpace(ViewState.PhoneE164))
                {
                    AddError(() => ViewState.PhoneInput, _localizationService.GetString(nameof(Autogenerated.Resources.GO_VE_PHONE_INVALID)));
                }
            }

            if (fieldIdentifier.FieldName == nameof(RegistrationPhoneViewState.TermsAccepted))
            {
                ClearError(() => ViewState.TermsAccepted);

                if (!ViewState.TermsAccepted)
                {
                    AddError(() => ViewState.TermsAccepted, _localizationService.GetString(nameof(Autogenerated.Resources.GO_VE_TERMS_REQUIRED)));
                }
            }
        }
    }
}
