using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Mappers.Registration;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
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

        public async Task<IReadOnlyList<RegistrationProvider>> GetProvidersAsync(
            CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<RegistrationsWebApiClient>();
            var providers = await client.GetProvidersAsync(cancellationToken);

            return providers
                .Select(RegistrationProviderMapper.Map)
                .ToList();
        }

        public async Task<RegistrationStartResult> StartAsync(
            RegistrationStartRequest request,
            CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<RegistrationsWebApiClient>();
            var model = RegistrationStartRequestMapper.Map(request);
            var result = await client.StartAsync(model, cancellationToken);
            return RegistrationStartResultMapper.Map(result);
        }

        public async Task<RegistrationCompleteResult> CompleteAsync(
            RegistrationCompleteRequest request,
            CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<RegistrationsWebApiClient>();
            var result = await client.CompleteAsync(new RegistrationCompleteModel
            {
                Token = request.Token,
                Profile = null!,
                Password = null // TODO возможно будет отправляться в конце регистрации
            }, cancellationToken);
            return RegistrationCompleteResultMapper.Map(result);
        }
    }
}
