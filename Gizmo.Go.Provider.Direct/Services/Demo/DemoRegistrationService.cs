using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using static Gizmo.Go.Core.Models.Registration.RegistrationChannel;

namespace Gizmo.Go.Provider.Direct.Services.Demo;

internal class DemoRegistrationService : IRegistrationService
{
    private static readonly IReadOnlyList<RegistrationProvider> Providers =
    [
        new RegistrationProvider
        {
            PublicId = new Guid("3877942f-bece-474a-bd10-e7187cfa419c"),
            Name = "Telegram bot",
            ChannelGuid = Telegram,
            CanRedirect = true,
            CanDispatchCode = true,
            CanProvidePhone = true,
            CanProvideEmail = false,
            HasChannel = false,
            Priority = true
        },
        new RegistrationProvider
        {
            PublicId = new Guid("6794cb03-8a6f-4860-8d8c-58d2629ce0b8"),
            Name = "SMS",
            ChannelGuid = Sms,
            CanRedirect = false,
            CanDispatchCode = true,
            CanProvidePhone = false,
            CanProvideEmail = false,
            HasChannel = false,
            Priority = true
        },
        new RegistrationProvider
        {
            PublicId = new Guid("c0d3d1aa-cd52-4d98-aef1-26a5b6b82bb4"),
            Name = "Flash Call",
            ChannelGuid = new Guid("f1a51100-0000-0000-0000-000000000001"),
            CanRedirect = false,
            CanDispatchCode = true,
            CanProvidePhone = false,
            CanProvideEmail = false,
            HasChannel = false,
            Priority = false
        },
        new RegistrationProvider
        {
            PublicId = new Guid("d6626ef7-68f0-4ea0-a816-8b74a39ab156"),
            Name = "Email",
            ChannelGuid = Email,
            CanRedirect = false,
            CanDispatchCode = true,
            CanProvidePhone = false,
            CanProvideEmail = false,
            HasChannel = false,
            Priority = false
        }
    ];

    public async Task<IReadOnlyList<RegistrationProvider>> GetProvidersAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1000, cancellationToken);

        return Providers;
    }

    public async Task<RegistrationStartResult> StartAsync(RegistrationStartRequest request, CancellationToken cancellationToken = default)
    {
        if (request.DeliveryMethod == RegistrationDeliveryMethod.Redirect)
        {
            await Task.Delay(500, cancellationToken);

            return new RegistrationStartResult
            {
                Result = RegistrationStartResultCode.Success,
                RedirectUrl = "https://t.me/GizmoGoBot?start=demo",
                Token = "demo-telegram-token"
            };
        }

        if (request.DeliveryMethod == RegistrationDeliveryMethod.CodeDispatch &&
            request.IntegrationPublicId == new Guid("6794cb03-8a6f-4860-8d8c-58d2629ce0b8"))
        {
            await Task.Delay(1000, cancellationToken);
            return new RegistrationStartResult
            {
                Result = RegistrationStartResultCode.Success,
                Token = "demo-sms-token",
                CodeLength = 6,
                ExpiresInSeconds = 120
            };
        }

        if (request.DeliveryMethod == RegistrationDeliveryMethod.CodeDispatch &&
            request.IntegrationPublicId == new Guid("d6626ef7-68f0-4ea0-a816-8b74a39ab156"))
        {
            await Task.Delay(1000, cancellationToken);
            return new RegistrationStartResult
            {
                Result = RegistrationStartResultCode.Success,
                Token = "demo-email-token",
                CodeLength = 6,
                ExpiresInSeconds = 120
            };
        }

        return new RegistrationStartResult { Result = RegistrationStartResultCode.Failed };
    }

    public async Task<RegistrationCompleteResult> CompleteAsync(RegistrationCompleteRequest request, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1000, cancellationToken);

        return new RegistrationCompleteResult
        {
            Result = RegistrationCompleteResultCode.Success
        };
    }

    public async Task<TokenConfirmedResult> IsTokenConfirmedAsync(string token, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1000, cancellationToken);
        return new TokenConfirmedResult { IsConfirmed = true, Phone = "+79173380454" };
    }
}
