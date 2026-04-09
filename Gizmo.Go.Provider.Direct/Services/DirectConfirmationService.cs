using Gizmo.Go.Core.Models.Confirmation;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Mappers.Confirmation;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.Provider.Direct.Services;

internal class DirectConfirmationService : IConfirmationService
{
    private readonly IServiceProvider _serviceProvider;

    public DirectConfirmationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task<TokenConfirmationResult> ConfirmAsync(TokenConfirmationRequest request, CancellationToken cancellationToken = default)
    {
        var client = _serviceProvider.GetRequiredService<TokensWebApiClient>();
        var model = new TokenConfirmModel
        {
            Token = request.Token,
            ConfirmationCode = request.ConfirmationCode
        };
        var result = await client.ConfirmAsync(model, cancellationToken);

        return TokenConfirmationResultMapper.Map(result);
    }
}