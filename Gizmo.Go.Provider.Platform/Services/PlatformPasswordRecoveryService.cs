using Gizmo.Go.Core.Models.PasswordRecovery;
using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformPasswordRecoveryService : IPasswordRecoveryService
    {
        public Task<IReadOnlyList<PasswordRecoveryProvider>> GetMethodsAsync(PasswordRecoveryIdentifierKind identifierKind, string value, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<PasswordRecoveryStartResult> StartAsync(PasswordRecoveryStartRequest request, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<PasswordRecoveryConfirmCode> ConfirmCodeAsync(string token, string confirmationCode, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<TokenConfirmedResult> IsTokenConfirmedAsync(string token, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<PasswordRecoveryCompleteCode> CompleteAsync(string token, string newPassword, CancellationToken ct = default) =>
            throw new NotImplementedException();
    }
}
