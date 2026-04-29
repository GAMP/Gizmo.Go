using Gizmo.Go.Core.Models.Confirmation;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Direct.Services.Demo;

internal class DemoConfirmationService : IConfirmationService
{
    public async Task<TokenConfirmationResult> ConfirmAsync(TokenConfirmationRequest request, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1000, cancellationToken);

        return new TokenConfirmationResult
        {
            Result = TokenConfirmationResultCode.Success
        };
    }
}
