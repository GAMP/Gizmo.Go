using Gizmo.Go.Core.Models.PasswordRecovery;
using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Mappers.Common;
using Gizmo.Go.Provider.Direct.Mappers.PasswordRecovery;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using UserRecoveriesClient = Gizmo.Web.Api.User.Clients.RecoveriesWebApiClient;
using UserTokensClient = Gizmo.Web.Api.User.Clients.TokensWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server password recovery service (open API).
    /// </summary>
    internal sealed class DirectPasswordRecoveryService : IPasswordRecoveryService
    {
        private readonly IServiceProvider _serviceProvider;

        public DirectPasswordRecoveryService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IReadOnlyList<PasswordRecoveryProvider>> GetMethodsAsync(PasswordRecoveryIdentifierKind identifierKind, string value, CancellationToken ct = default)
        {
            var client = _serviceProvider.GetRequiredService<UserRecoveriesClient>();
            var model = new GetRecoveryMethodsModel
            {
                Value = value,
                ValueKind = PasswordRecoveryStartRequestMapper.ToValueKind(identifierKind)
            };
            var raw = await client.GetMethodsAsync(model, ct);
            return raw.Select(PasswordRecoveryProviderMapper.Map).ToList();
        }

        public async Task<PasswordRecoveryStartResult> StartAsync(PasswordRecoveryStartRequest request, CancellationToken ct = default)
        {
            var client = _serviceProvider.GetRequiredService<UserRecoveriesClient>();
            var model = PasswordRecoveryStartRequestMapper.Map(request);
            var result = await client.PasswordRecoveryStartAsync(model, ct);
            return PasswordRecoveryStartResultMapper.Map(result, request);
        }

        public async Task<PasswordRecoveryConfirmCode> ConfirmCodeAsync(string token, string confirmationCode, CancellationToken ct = default)
        {
            var client = _serviceProvider.GetRequiredService<UserTokensClient>();
            var result = await client.ConfirmAsync(new TokenConfirmModel
            {
                Token = token,
                ConfirmationCode = confirmationCode
            }, ct);

            return PasswordRecoveryConfirmCodeMapper.Map(result);
        }

        /// <remarks>
        /// Polls the registrations token endpoint - the api exposes no recovery specific
        /// "confirmed" route. Only reachable for redirect/call recovery methods, which no
        /// current server configuration returns, so the endpoint has not been verified
        /// against a password recovery token.
        /// </remarks>
        public async Task<TokenConfirmedResult> IsTokenConfirmedAsync(string token, CancellationToken ct = default)
        {
            var client = _serviceProvider.GetRequiredService<RegistrationsWebApiClient>();
            var result = await client.ConfirmedAsync(new TokenCheckModel { Token = token }, ct);
            return TokenConfirmedResultMapper.Map(result);
        }

        public async Task<PasswordRecoveryCompleteCode> CompleteAsync(string token, string newPassword, CancellationToken ct = default)
        {
            // anonymous on the server, no token is needed
            var client = _serviceProvider.GetRequiredService<VerificationCompleteWebApiClient>();
            var result = await client.RecoveryCompleteAsync(new PasswordRecoveryCompleteModel
            {
                Token = token,
                NewPassword = newPassword
            }, ct);

            return PasswordRecoveryCompleteCodeMapper.Map(result);
        }
    }
}
