using Gizmo.Go.Core.Services;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.Services.Common
{
    public sealed class AuthOptionsService : IAuthOptionsService
    {
        public AuthOptionsService(
            ILogger<AuthOptionsService> logger,
            IMobileOptionsService mobileOptionsService,
            IUserRegistrationService registrationService)
        {
            _logger = logger;
            _mobileOptionsService = mobileOptionsService;
            _registrationService = registrationService;
        }

        // a screen must not wait for an unreachable server for the default HTTP timeout
        private static readonly TimeSpan LoadTimeout = TimeSpan.FromSeconds(5);

        private readonly ILogger<AuthOptionsService> _logger;
        private readonly IMobileOptionsService _mobileOptionsService;
        private readonly IUserRegistrationService _registrationService;

        private AuthOptions? _cached;

        public async Task<AuthOptions?> GetAsync(CancellationToken cancellationToken = default)
        {
            if (_cached is not null)
                return _cached;

            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(LoadTimeout);

            try
            {
                var options = await _mobileOptionsService.GetAsync(timeout.Token);

                bool isRegistrationEnabled;
                try
                {
                    // null: the server has no default user group, so there is nothing to register into
                    var requiredInfo = await _registrationService.GetRequiredUserInfoAsync(timeout.Token);
                    isRegistrationEnabled = options.IsRegistrationEnabled && requiredInfo is not null;
                }
                catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
                {
                    isRegistrationEnabled = false;
                    _logger.LogError(ex, "Could not determine if registration is enabled.");
                }

                _cached = new AuthOptions()
                {
                    IsRegistrationEnabled = isRegistrationEnabled,
                    IsPasswordRecoveryEnabled = options.IsPasswordRecoveryEnabled,
                    LogoUrl = options.LogoUrl,
                    CountryCode = options.CountryCode,
                };

                return _cached;
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                // no silent fallback: the screens hide registration and recovery and show a connection error
                _logger.LogError(ex, "Could not load the mobile options.");
                return null;
            }
        }
    }
}
