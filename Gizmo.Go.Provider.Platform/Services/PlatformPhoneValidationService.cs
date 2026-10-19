using Gizmo.Go.Core.Models.PhoneValidation;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformPhoneValidationService : IPhoneValidationService
    {
        public Task<IReadOnlyList<PhoneCountry>> GetCountriesAsync(CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<PhoneValidationResult> ValidateAsync(string phoneNumber, string regionCode, CancellationToken ct = default) =>
            throw new NotImplementedException();
    }
}
