using Gizmo.Go.Core.Models.Branch;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Mappers.Branch;
using Gizmo.Server.Exceptions;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BranchesClient = Gizmo.Web.Api.User.Clients.BranchesWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server branch provider (api/user/v3/branches, secured).
    /// </summary>
    internal sealed class DirectBranchProvider : IBranchProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DirectBranchProvider> _logger;

        public DirectBranchProvider(IServiceProvider serviceProvider, ILogger<DirectBranchProvider> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<Branch>> GetAsync(CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<BranchesClient>();
            var request = new UserBranchesFilter
            {
                Pagination = new ModelFilterPagination { Limit = -1 },
            };

            var page = await client.GetAsync(request, cancellationToken);
            return page.Data.Select(branch => BranchMapper.Map(branch, _logger)).ToList();
        }

        public async Task<BranchDetails?> GetDetailsAsync(int branchId, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _serviceProvider.GetRequiredService<BranchesClient>();
                return BranchMapper.Map(await client.GetByIdAsync(branchId, cancellationToken), _logger);
            }
            catch (Exception ex) when (ex.IsExceptionCode(ExceptionCode.EntityNotFound))
            {
                return null;
            }
        }
    }
}
