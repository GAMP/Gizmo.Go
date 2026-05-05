using System.Globalization;
using System.Web;
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
    [Route(NavigationHelper.RegistrationCallPhone)]
    public sealed class RegistrationCallPhoneViewService : ValidatingViewStateServiceBase<RegistrationCallPhoneViewState>
    {
        private readonly NavigationService _navigationService;
        private readonly IPhoneValidationService _phoneValidationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly ILocalizationService _localizationService;

        public RegistrationCallPhoneViewService(
            RegistrationCallPhoneViewState viewState,
            ILogger<RegistrationCallPhoneViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IPhoneValidationService phoneValidationService,
            IRegistrationSessionService registrationSession,
            ILocalizationService localizationService)
            : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _phoneValidationService = phoneValidationService;
            _registrationSession = registrationSession;
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

            const int separatorBuffer = 6;

            ViewState.PhoneInput = phoneInput;
            ViewState.PhoneE164 = result.E164;
            ViewState.FormattedPhoneInput = result.FormattedNational;
            ViewState.SelectedCountryIso2 = country?.Iso2 ?? string.Empty;
            ViewState.SelectedCountryFlag = country?.Flag ?? string.Empty;
            ViewState.SelectedDialCode = country?.DialCode ?? string.Empty;
            ViewState.SelectedCountryPlaceholder = country?.Placeholder ?? string.Empty;
            ViewState.PhoneLength = maxDigits + separatorBuffer;

            ValidateProperty(() => ViewState.PhoneInput);
            ViewState.RaiseChanged();

            return ValueTask.CompletedTask;
        }

        public ValueTask SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
            {
                ViewState.RaiseChanged();
                return ValueTask.CompletedTask;
            }

            //TODO: вызов API для call-based верификации когда будет реализован
            _registrationSession.SetPhone(ViewState.PhoneE164);
            _registrationSession.SetFlow(RegistrationFlow.Call);
            _navigationService.NavigateTo(NavigationHelper.RegistrationCallVerify);

            return ValueTask.CompletedTask;
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
                return;
            }

            ViewState.IntegrationPublicId = channelId;

            const int separatorBuffer = 6;
            var culture = CultureInfo.CurrentUICulture.Name;
            var parts = culture.Split('-');
            var defaultIso2 = parts.Length > 1 ? parts[^1] : "US";
            var defaultCountry = PhoneCountryList.FindByIso2(defaultIso2);

            ViewState.PhoneInput = string.Empty;
            ViewState.PhoneE164 = string.Empty;
            ViewState.FormattedPhoneInput = string.Empty;
            ViewState.SelectedCountryIso2 = defaultCountry?.Iso2 ?? string.Empty;
            ViewState.SelectedCountryFlag = defaultCountry?.Flag ?? string.Empty;
            ViewState.SelectedDialCode = defaultCountry?.DialCode ?? string.Empty;
            ViewState.SelectedCountryPlaceholder = defaultCountry?.Placeholder ?? string.Empty;
            ViewState.PhoneLength = _phoneValidationService.GetMaxLength(defaultIso2) + separatorBuffer;

            ClearError(() => ViewState.PhoneInput);

            ViewState.RaiseChanged();

            await base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger trigger)
        {
            if (fieldIdentifier.FieldName
                is nameof(RegistrationCallPhoneViewState.PhoneInput)
                or nameof(RegistrationCallPhoneViewState.SelectedCountryIso2))
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
        }
    }
}
