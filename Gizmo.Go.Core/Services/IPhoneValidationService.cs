using Gizmo.Go.Core.Models.PhoneValidation;

namespace Gizmo.Go.Core.Services
{
    public interface IPhoneValidationService
    {
        Task<IReadOnlyList<PhoneCountry>> GetCountriesAsync(CancellationToken ct = default);
        Task<PhoneValidationResult> ValidateAsync(string phoneNumber, string regionCode, CancellationToken ct = default);
    }
}
