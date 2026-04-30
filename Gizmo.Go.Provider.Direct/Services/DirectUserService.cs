using Gizmo.Go.Core.Services;
using Gizmo.Web.Api.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.Provider.Direct.Services
{
    internal sealed class DirectUserService : IUserService
    {
        private readonly IServiceProvider _serviceProvider;

        public DirectUserService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<UsersWebApiClient>();
            var result = await client.UsernameExistAsync(username, cancellationToken);
            return result.Id != null;
        }
    }
}
