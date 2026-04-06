using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Mappers.Registration;
using Gizmo.Web.Api.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.Provider.Direct.Services
{
    internal sealed class DirectRegistrationService : IRegistrationService
    {
        private readonly IServiceProvider _serviceProvider;

        public DirectRegistrationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IReadOnlyList<VerificationProvider>> GetProvidersAsync(
            CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<RegistrationsWebApiClient>();
            var providers = await client.GetProvidersAsync(cancellationToken);
            
            return providers
                .Select(VerificationProviderMapper.Map)
                .ToList();
        }
    }
}
