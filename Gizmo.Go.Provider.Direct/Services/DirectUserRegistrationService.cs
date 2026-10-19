using Gizmo.Go.Core.Configuration;
using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Mappers.Common;
using Gizmo.Go.Provider.Direct.Mappers.Registration;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserAgreementsClient = Gizmo.Web.Api.User.Clients.UserAgreementsWebApiClient;
using UserTokensClient = Gizmo.Web.Api.User.Clients.TokensWebApiClient;
using UserUsersClient = Gizmo.Web.Api.User.Clients.UsersWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server user registration service (open API).
    /// </summary>
    internal sealed class DirectUserRegistrationService : IUserRegistrationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<RegistrationStubSettings> _stubSettings;
        private readonly ILogger<DirectUserRegistrationService> _logger;
        private IReadOnlyList<RegistrationProvider>? _cachedMethods;

        public DirectUserRegistrationService(
            IServiceProvider serviceProvider,
            IOptions<RegistrationStubSettings> stubSettings,
            ILogger<DirectUserRegistrationService> logger)
        {
            _serviceProvider = serviceProvider;
            _stubSettings = stubSettings;
            _logger = logger;
        }

        public async Task<IReadOnlyList<RegistrationProvider>> GetMethodsAsync(CancellationToken ct = default)
        {
            if (_cachedMethods is not null)
                return _cachedMethods;

            var client = _serviceProvider.GetRequiredService<RegistrationsWebApiClient>();
            var methods = await client.GetMethodsAsync(ct);
            _cachedMethods = methods.Select(RegistrationProviderMapper.Map).ToList();
            return _cachedMethods;
        }

        public Task<RegistrationRequiredInfo?> GetRequiredUserInfoAsync(CancellationToken ct = default)
        {
            // stub: default-group required info is not in the open API yet (client reads it over RPC)
            var info = _stubSettings.Value.RequiredInfo;

            return Task.FromResult<RegistrationRequiredInfo?>(new RegistrationRequiredInfo
            {
                FirstName = info.FirstName,
                LastName = info.LastName,
                BirthDate = info.BirthDate,
                Address = info.Address,
                City = info.City,
                PostCode = info.PostCode,
                State = info.State,
                Country = info.Country,
                Email = info.Email,
                Phone = info.Phone,
                Mobile = info.Mobile,
                Sex = info.Sex,
                Password = true,
            });
        }

        public async Task<IReadOnlyList<RegistrationAgreement>> GetAgreementsAsync(CancellationToken ct = default)
        {
            try
            {
                var client = _serviceProvider.GetRequiredService<UserAgreementsClient>();
                var result = await client.GetAsync(new PublicUserAgreementsFilter(), ct);
                if (result.Data is null)
                    return Array.Empty<RegistrationAgreement>();
                return result.Data.Select(RegistrationAgreementMapper.Map).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetAgreementsAsync failed, returning empty list");
                return Array.Empty<RegistrationAgreement>();
            }
        }

        public async Task<bool> UserNameExistAsync(string userName, CancellationToken ct = default)
        {
            var client = _serviceProvider.GetRequiredService<UserUsersClient>();
            return await client.UsernameExistAsync(userName, ct);
        }

        public async Task<bool> EmailExistAsync(string email, CancellationToken ct = default)
        {
            var client = _serviceProvider.GetRequiredService<UserUsersClient>();
            return await client.EmailExistAsync(email, ct);
        }

        public async Task<bool> MobilePhoneExistAsync(string mobilePhone, CancellationToken ct = default)
        {
            var client = _serviceProvider.GetRequiredService<UserUsersClient>();
            return await client.MobilePhoneExistAsync(mobilePhone, ct);
        }

        public async Task<RegistrationStartResult> StartAsync(RegistrationStartRequest request, CancellationToken ct = default)
        {
            var client = _serviceProvider.GetRequiredService<RegistrationsWebApiClient>();
            var model = RegistrationStartRequestMapper.Map(request);
            var result = await client.StartAsync(model, ct);
            return RegistrationStartResultMapper.Map(result, request);
        }

        public async Task<RegistrationConfirmCode> ConfirmTokenAsync(string token, string confirmationCode, CancellationToken ct = default)
        {
            var client = _serviceProvider.GetRequiredService<UserTokensClient>();
            return RegistrationConfirmCodeMapper.Map(await client.ConfirmAsync(new TokenConfirmModel { Token = token, ConfirmationCode = confirmationCode }, ct));
        }

        public async Task<TokenConfirmedResult> IsTokenConfirmedAsync(string token, CancellationToken ct = default)
        {
            var client = _serviceProvider.GetRequiredService<RegistrationsWebApiClient>();
            var result = await client.ConfirmedAsync(new TokenCheckModel { Token = token }, ct);
            return TokenConfirmedResultMapper.Map(result);
        }

        public async Task<RegistrationCompleteCode> CompleteAsync(RegistrationCompleteRequest request, CancellationToken ct = default)
        {
            var client = _serviceProvider.GetRequiredService<RegistrationsWebApiClient>();
            var model = RegistrationCompleteRequestMapper.MapComplete(request);
            var result = await client.CompleteAsync(model, ct);
            return RegistrationCompleteCodeMapper.Map(result);
        }

        public async Task<RegistrationCompleteCode> DirectAsync(RegistrationCompleteRequest request, CancellationToken ct = default)
        {
            var client = _serviceProvider.GetRequiredService<RegistrationsWebApiClient>();
            var model = RegistrationCompleteRequestMapper.MapDirect(request);
            var result = await client.DirectAsync(model, ct);
            return RegistrationCompleteCodeMapper.Map(result);
        }
    }
}
