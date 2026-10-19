using Gizmo.Go.Core.Models.PhoneValidation;
using Gizmo.Go.Core.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ValidationsClient = Gizmo.Web.Api.User.Clients.ValidationsWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server phone validation service (open API).
    /// </summary>
    internal sealed class DirectPhoneValidationService : IPhoneValidationService
    {
        // UI resource key names, resolved by the UI layer.
        private const string ErrorOccurredKey = "GO_GEN_AN_ERROR_HAS_OCCURRED";
        private const string PhoneInvalidKey = "GO_REGISTRATION_VE_MOBILE_PHONE_INVALID";

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DirectPhoneValidationService> _logger;
        private IReadOnlyList<PhoneCountry>? _cachedCountries;

        public DirectPhoneValidationService(IServiceProvider serviceProvider, ILogger<DirectPhoneValidationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<IReadOnlyList<PhoneCountry>> GetCountriesAsync(CancellationToken ct = default)
        {
            if (_cachedCountries is not null)
                return _cachedCountries;

            try
            {
                var client = _serviceProvider.GetRequiredService<ValidationsClient>();
                var models = await client.GetPhoneCountriesAsync(ct);
                _cachedCountries = models.Select(m => new PhoneCountry
                {
                    RegionCode = m.RegionCode,
                    CountryName = m.CountryName,
                    CallingCode = m.CallingCode,
                    InputMask = m.InputMask,
                    Placeholder = m.Placeholder,
                    MaxLength = m.MaxLength,
                    Flag = m.Flag,
                }).ToList();
                return _cachedCountries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load phone countries from server.");
                return Array.Empty<PhoneCountry>();
            }
        }

        public async Task<PhoneValidationResult> ValidateAsync(string phoneNumber, string regionCode, CancellationToken ct = default)
        {
            try
            {
                var client = _serviceProvider.GetRequiredService<ValidationsClient>();
                var result = await client.ValidatePhoneAsync(new PhoneValidationRequestModel
                {
                    PhoneNumber = phoneNumber,
                    RegionCode = regionCode,
                }, ct);

                if (result.IsValid)
                    return new PhoneValidationResult { IsValid = true, E164 = result.E164 };

                var errorKey = result.Result == PhoneValidationResultCode.Error
                    ? ErrorOccurredKey
                    : PhoneInvalidKey;

                return new PhoneValidationResult { IsValid = false, ErrorKey = errorKey };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate phone number.");
                return new PhoneValidationResult { IsValid = false, ErrorKey = ErrorOccurredKey };
            }
        }
    }
}
