using Gizmo.Go.Core.Models.Confirmation;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services;

/// <summary>
/// Stub confirmation service for local UI development.
/// Accepts confirmation code "1234". Any other code returns InvalidConfirmationCode.
/// </summary>
internal sealed class StubConfirmationService : IConfirmationService
{
    private const string ValidCode = "1234";

    public Task<TokenConfirmationResult> ConfirmAsync(
        TokenConfirmationRequest request,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[STUB] ConfirmAsync — token: {request.Token}  code: {request.ConfirmationCode}");

        var code = request.ConfirmationCode == ValidCode
            ? TokenConfirmationResultCode.Success
            : TokenConfirmationResultCode.InvalidConfirmationCode;

        return Task.FromResult(new TokenConfirmationResult { Result = code });
    }
}
