using Gizmo.Go.Core.Configuration;
using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using Microsoft.Extensions.Options;

namespace Gizmo.Go.Provider.Direct.Services
{
    // stub: registration options are not in the open API yet (client reads them over RPC)
    internal sealed class DirectRegistrationOptionsService : IRegistrationOptionsService
    {
        private readonly IOptions<RegistrationStubSettings> _settings;

        public DirectRegistrationOptionsService(IOptions<RegistrationStubSettings> settings)
        {
            _settings = settings;
        }

        public Task<RegistrationOptions> GetAsync(CancellationToken ct = default)
        {
            var settings = _settings.Value;

            return Task.FromResult(new RegistrationOptions
            {
                IsRegistrationEnabled = settings.IsRegistrationEnabled,
                IsDirectRegistrationEnabled = settings.IsDirectRegistrationEnabled,
                IsPasswordRecoveryEnabled = settings.IsPasswordRecoveryEnabled
            });
        }
    }
}
