using Gizmo.Go.Core.Models.Agreements;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Mappers.Agreements;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using PendingUserAgreementsClient = Gizmo.Web.Api.User.Clients.PendingUserAgreementsWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server user agreements pending for the current user (api/user/v3/useragreements).
    /// </summary>
    internal sealed class DirectPendingUserAgreementsService : IPendingUserAgreementsService
    {
        private readonly IServiceProvider _serviceProvider;

        public DirectPendingUserAgreementsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IReadOnlyList<PendingUserAgreement>> GetPendingAsync(CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<PendingUserAgreementsClient>();
            var result = await client.GetPendingAsync(new UserAgreementsFilter(), cancellationToken);

            if (result.Data is null)
                return Array.Empty<PendingUserAgreement>();

            return result.Data.Select(PendingUserAgreementMapper.Map).ToList();
        }

        public async Task AcceptAsync(int userAgreementId, CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<PendingUserAgreementsClient>();
            await client.AcceptAsync(userAgreementId, cancellationToken);
        }
    }
}
