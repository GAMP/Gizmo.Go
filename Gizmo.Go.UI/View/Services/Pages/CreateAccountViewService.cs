using System.Globalization;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.View.Models;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhoneNumbers;

namespace Gizmo.Go.UI.View.Services.Pages
{
    [Register()]
    [Route(NavigationHelper.CreateAccount)]
    public sealed class CreateAccountViewService : ValidatingViewStateServiceBase<CreateAccountViewState>
    {
        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;

        public CreateAccountViewService(
            CreateAccountViewState viewState,
            ILogger<CreateAccountViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
        }

        #endregion

        #region METHODS

        public ValueTask UpdatePhoneAsync(string phoneInput, string countryIso2)
        {
var isFreeMode = string.IsNullOrEmpty(countryIso2);
            var country = isFreeMode ? null : PhoneCountryList.FindByIso2(countryIso2);

            var e164 = string.Empty;
            var formattedLocal = string.Empty;

            if (!string.IsNullOrWhiteSpace(phoneInput))
            {
                if (isFreeMode)
                {
                    e164 = phoneInput;
                }
                else if (country is not null)
                {
                    var util = PhoneNumberUtil.GetInstance();
                    try
                    {
                        var parsed = util.Parse(phoneInput, country.Iso2.ToUpperInvariant());

                        if (util.IsValidNumber(parsed))
                        {
                            e164 = util.Format(parsed, PhoneNumberFormat.E164);
                            formattedLocal = FormatNationalLocal(parsed, country.Iso2);
                        }
                    }
                    catch (NumberParseException)
                    {
                        //TODO подумать о бизнес ошибках
                    }
                }
            }

            const int separatorBuffer = 6; //TODO временное решение
            var maxDigits = 15;

            if (country is not null)
            {
                var metaUtil = PhoneNumberUtil.GetInstance();
                var metaData = metaUtil.GetMetadataForRegion(country.Iso2.ToUpperInvariant());

                if (metaData?.GeneralDesc.PossibleLengthList.Count > 0)
                {
                    maxDigits = metaData.GeneralDesc.PossibleLengthList.Max();
                }
            }
            
            ViewState.PhoneInput = phoneInput;
            ViewState.PhoneE164 = e164;
            ViewState.FormattedPhoneInput = formattedLocal;
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

        public ValueTask SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
            {
                ViewState.RaiseChanged();
                return ValueTask.CompletedTask;
            }

            var encodedPhone = Uri.EscapeDataString(ViewState.PhoneE164);
            _navigationService.NavigateTo($"{NavigationHelper.ConfirmationPage}?phone={encodedPhone}");
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateBackAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.LoginPage);
            return ValueTask.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            const int separatorBuffer = 6; //TODO временное решение
            var culture = CultureInfo.CurrentUICulture.Name;
            var parts = culture.Split('-');
            var defaultIso2 = parts.Length > 1 ? parts[^1] : "US";
            var defaultCountry = PhoneCountryList.FindByIso2(defaultIso2);
            
            var navUtil = PhoneNumberUtil.GetInstance();
            var navMetadata = defaultCountry is not null
                ? navUtil.GetMetadataForRegion(defaultCountry.Iso2.ToUpperInvariant())
                : null;
            
            ViewState.PhoneInput = string.Empty;
            ViewState.PhoneE164 = string.Empty;
            ViewState.FormattedPhoneInput = string.Empty;
            ViewState.SelectedCountryIso2 = defaultCountry?.Iso2 ?? string.Empty;
            ViewState.SelectedCountryName = defaultCountry?.Name ?? string.Empty;
            ViewState.SelectedDialCode = defaultCountry?.DialCode ?? string.Empty;
            ViewState.SelectedCountryPlaceholder = defaultCountry?.Placeholder ?? string.Empty;
            ViewState.PhoneLength = navMetadata?.GeneralDesc.PossibleLengthList.Count > 0
                ? navMetadata.GeneralDesc.PossibleLengthList.Max() + separatorBuffer
                : 15 + separatorBuffer;
            ViewState.TermsAccepted = false;
            
            ViewState.RaiseChanged();
            
            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger trigger)
        {
            if (fieldIdentifier.FieldName 
                is nameof(CreateAccountViewState.PhoneInput) 
                or nameof(CreateAccountViewState.SelectedCountryIso2))
            {
                ClearError(() => ViewState.PhoneInput);

                if (string.IsNullOrWhiteSpace(ViewState.SelectedCountryIso2))
                {
                    if (string.IsNullOrWhiteSpace(ViewState.PhoneInput))
                    {
                        AddError(() => ViewState.PhoneInput, Autogenerated.Resources.GO_VE_PHONE_REQUIRED);
                    }

                    return;
                }

                if (string.IsNullOrWhiteSpace(ViewState.PhoneInput))
                {
                    AddError(() => ViewState.PhoneInput, Autogenerated.Resources.GO_VE_PHONE_INVALID);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ViewState.PhoneE164))
                {
                    AddError(() => ViewState.PhoneInput, Autogenerated.Resources.GO_VE_PHONE_INVALID);
                }
            }

            if (fieldIdentifier.FieldName == nameof(CreateAccountViewState.TermsAccepted))
            {
                ClearError(() => ViewState.TermsAccepted);

                if (!ViewState.TermsAccepted)
                {
                    AddError(() => ViewState.TermsAccepted, Autogenerated.Resources.GO_VE_TERMS_REQUIRED);
                }
            }
        }
        
        #endregion

        #region HELPERS

        private static string FormatNationalLocal(PhoneNumber parsed, string iso2)
        {
            var util = PhoneNumberUtil.GetInstance();
            var national = util.Format(parsed, PhoneNumberFormat.NATIONAL);
            var ndd = util.GetNddPrefixForRegion(iso2.ToUpperInvariant(), true);

            if (!string.IsNullOrEmpty(ndd) && national.StartsWith(ndd))
            {
                national = national[ndd.Length..].TrimStart();
            }

            return national;
        }

        #endregion
    }
}
