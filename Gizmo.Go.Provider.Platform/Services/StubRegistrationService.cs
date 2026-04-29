using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services;

/// <summary>
/// Stub registration service for local UI development.
/// No backend required. SMS code is always "1234".
/// </summary>
internal sealed class StubRegistrationService : IRegistrationService
{
    private static readonly Guid _stubProviderGuid = new("00000000-0000-0000-0000-000000000001");

    public Task<IReadOnlyList<RegistrationProvider>> GetProvidersAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<RegistrationProvider> providers =
        [
            new RegistrationProvider
            {
                PublicId         = _stubProviderGuid,
                Name             = "SMS (stub)",
                ChannelGuid      = _stubProviderGuid,
                CanDispatchCode  = true,
                CanProvidePhone  = true,
                HasChannel       = true,
            }
        ];

        return Task.FromResult(providers);
    }

    public Task<RegistrationStartResult> StartAsync(
        RegistrationStartRequest request,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine("[STUB] RegistrationStartAsync called.");
        Console.WriteLine($"[STUB] Phone: {request.PhoneNumber}  Email: {request.Email}");
        Console.WriteLine("[STUB] Confirmation code: 1234");

        var result = new RegistrationStartResult
        {
            Result           = RegistrationStartResultCode.Success,
            Token            = "stub-token-" + Guid.NewGuid().ToString("N")[..8],
            CodeLength       = 4,
            ExpiresInSeconds = 300,
        };

        return Task.FromResult(result);
    }

    public Task<RegistrationCompleteResult> CompleteAsync(
        RegistrationCompleteRequest request,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine("[STUB] RegistrationCompleteAsync called.");
        Console.WriteLine($"[STUB] Token: {request.Token}  Username: {request.Profile.Username}");

        var result = new RegistrationCompleteResult
        {
            Result = RegistrationCompleteResultCode.Success
        };

        return Task.FromResult(result);
    }
}
