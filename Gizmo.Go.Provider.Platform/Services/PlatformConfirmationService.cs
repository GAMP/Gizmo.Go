using Gizmo.Go.Core.Models.Confirmation;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services;

public class PlatformConfirmationService : IConfirmationService
{
    public Task<TokenConfirmationResult> ConfirmAsync(TokenConfirmationRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}