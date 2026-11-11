using Gizmo.Go.Core.Models.Agreements;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Mappers.Agreements;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using UserAgreementsClient = Gizmo.Web.Api.User.Clients.UserAgreementsWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server public user agreements (api/user/v3/useragreements, unsecured).
    /// </summary>
    internal sealed class DirectUserAgreementsService : IUserAgreementsService
    {
        private readonly IServiceProvider _serviceProvider;

        public DirectUserAgreementsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IReadOnlyList<UserAgreement>> GetAsync(CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<UserAgreementsClient>();
            var filter = new PublicUserAgreementsFilter
            {
                Pagination = new ModelFilterPagination { Limit = -1 },
            };

            var page = await client.GetAsync(filter, cancellationToken);

            return page.Data
                .Select(UserAgreementMapper.Map)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Id)
                .ToList();
        }
    }
}
