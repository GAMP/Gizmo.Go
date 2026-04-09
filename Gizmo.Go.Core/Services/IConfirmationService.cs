using Gizmo.Go.Core.Models.Confirmation;

namespace Gizmo.Go.Core.Services;

public interface IConfirmationService
{
    Task<TokenConfirmationResult> ConfirmAsync(
        TokenConfirmationRequest request,
        CancellationToken cancellationToken = default);
}