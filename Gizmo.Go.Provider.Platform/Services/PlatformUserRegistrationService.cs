using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformUserRegistrationService : IUserRegistrationService
    {
        public Task<IReadOnlyList<RegistrationProvider>> GetMethodsAsync(CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<RegistrationRequiredInfo?> GetRequiredUserInfoAsync(CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<RegistrationAgreement>> GetAgreementsAsync(CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<bool> UserNameExistAsync(string userName, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<bool> EmailExistAsync(string email, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<bool> MobilePhoneExistAsync(string mobilePhone, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<RegistrationStartResult> StartAsync(RegistrationStartRequest request, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<RegistrationConfirmCode> ConfirmTokenAsync(string token, string confirmationCode, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<TokenConfirmedResult> IsTokenConfirmedAsync(string token, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<RegistrationCompleteCode> CompleteAsync(RegistrationCompleteRequest request, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<RegistrationCompleteCode> DirectAsync(RegistrationCompleteRequest request, CancellationToken ct = default) =>
            throw new NotImplementedException();
    }
}
