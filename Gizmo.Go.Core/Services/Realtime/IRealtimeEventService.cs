using Gizmo.Web.Api.Messaging;

namespace Gizmo.Go.Core.Services.Realtime
{
    public interface IRealtimeEventService
    {
        bool IsConnected { get; }

        Task ConnectAsync(string accessToken, CancellationToken ct = default);

        Task DisconnectAsync(CancellationToken ct = default);

        IDisposable Subscribe<TMessage>(Action<TMessage> handler)
            where TMessage : class, IAPIEventMessage;
    }
}
