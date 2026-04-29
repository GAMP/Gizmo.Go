using Gizmo.Go.Core.Services.Realtime;
using Gizmo.Web.Api.Messaging;
using System.Collections.Concurrent;

namespace Gizmo.Go.Provider.Direct.Services.Demo;

internal sealed class DemoRealtimeEventService : IRealtimeEventService
{
    #region FIELDS

    private const int DemoEventDelayMs = 3000;

    private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

    #endregion

    #region PROPERTIES

    public bool IsConnected => false;

    #endregion

    #region METHODS

    public Task ConnectAsync(string accessToken, CancellationToken ct = default) => Task.CompletedTask;

    public Task DisconnectAsync(CancellationToken ct = default) => Task.CompletedTask;

    public IDisposable Subscribe<TMessage>(Action<TMessage> handler)
        where TMessage : class, IAPIEventMessage
    {
        var list = _handlers.GetOrAdd(typeof(TMessage), _ => new List<Delegate>());
        lock (list)
        {
            list.Add(handler);
        }

        if (typeof(TMessage) == typeof(UserCreatedEventMessage))
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(DemoEventDelayMs);
                Fire(new UserCreatedEventMessage { UserId = 0 });
            });
        }

        return new Subscription(() =>
        {
            lock (list)
            {
                list.Remove(handler);
            }
        });
    }

    #endregion

    #region PRIVATE

    private void Fire(IAPIEventMessage message)
    {
        var messageType = message.GetType();

        foreach (var (type, handlers) in _handlers)
        {
            if (!type.IsAssignableFrom(messageType))
                continue;

            Delegate[] snapshot;
            lock (handlers)
            {
                snapshot = handlers.ToArray();
            }

            foreach (var h in snapshot)
            {
                h.DynamicInvoke(message);
            }
        }
    }

    private sealed class Subscription : IDisposable
    {
        private readonly Action _onDispose;

        public Subscription(Action onDispose) => _onDispose = onDispose;

        public void Dispose() => _onDispose();
    }

    #endregion
}
