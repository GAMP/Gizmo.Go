using Gizmo.Go.Core.Models.News;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Mappers.News;
using Gizmo.Server.Exceptions;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using NewsClient = Gizmo.Web.Api.User.Clients.NewsWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server news (api/user/v3/news, secured).
    /// </summary>
    internal sealed class DirectNewsService : INewsService
    {
        private readonly IServiceProvider _serviceProvider;

        public DirectNewsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IReadOnlyList<NewsItem>> GetListAsync(int? limit = null, CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<NewsClient>();
            var filter = new NewsFilter
            {
                Pagination = new ModelFilterPagination { Limit = limit ?? -1, SortBy = "Id", IsAsc = false },
            };

            var page = await client.GetAsync(filter, cancellationToken);
            return page.Data.Select(NewsItemMapper.Map).ToList();
        }

        public async Task<NewsItem?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _serviceProvider.GetRequiredService<NewsClient>();
                return NewsItemMapper.Map(await client.GetByIdAsync(id, cancellationToken));
            }
            catch (Exception ex) when (ex.IsExceptionCode(ExceptionCode.EntityNotFound))
            {
                return null;
            }
        }
    }
}
