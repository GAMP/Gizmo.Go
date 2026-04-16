using Gizmo.Go.Core.Configuration;
using Gizmo.Go.Core.Services.Realtime;
using Gizmo.Web.Api;
using Gizmo.Web.Api.Messaging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Gizmo.Go.UI.Services.Realtime
{
    internal sealed class SignalRRealtimeEventService : IRealtimeEventService
    {
        #region FIELDS

        private readonly IOptions<GizmoGoSettings> _settings;
        private readonly NavigationManager _navigationManager;
        private readonly ILogger<SignalRRealtimeEventService> _logger;

        private HubConnection? _connection;
        private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

        #endregion

        #region CONSTRUCTOR

        public SignalRRealtimeEventService(
            IOptions<GizmoGoSettings> settings,
            NavigationManager navigationManager,
            ILogger<SignalRRealtimeEventService> logger)
        {
            _settings = settings;
            _navigationManager = navigationManager;
            _logger = logger;
        }

        #endregion

        #region PROPERTIES

        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        #endregion

        #region METHODS

        public async Task ConnectAsync(string accessToken, CancellationToken ct = default)
        {
            if (_connection != null)
                await DisconnectAsync(ct);

            var baseUrl = _settings.Value.BackendUrl;
            if (string.IsNullOrWhiteSpace(baseUrl))
                baseUrl = _navigationManager.BaseUri;

            var hubUrl = $"{baseUrl.TrimEnd('/')}/api/events";

            try
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl(hubUrl, options =>
                    {
                        options.Transports = HttpTransportType.WebSockets;
                        options.AccessTokenProvider = () => Task.FromResult<string?>(accessToken);
                    })
                    .AddJsonProtocol(options =>
                    {
                        options.PayloadSerializerOptions.Converters.Add(
                            new MessagePackUnionMessageJsonConverter<IAPIEventMessage>("EventId", "Event"));
                    })
                    .Build();

                _connection.On<IAPIEventMessage>("Event", OnEventReceived);

                await _connection.StartAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to realtime events hub at {Url}", hubUrl);
            }
        }

        public async Task DisconnectAsync(CancellationToken ct = default)
        {
            if (_connection == null)
                return;

            try
            {
                await _connection.StopAsync(ct);
                await _connection.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while disconnecting from realtime events hub");
            }
            finally
            {
                _connection = null;
            }
        }

        public IDisposable Subscribe<TMessage>(Action<TMessage> handler)
            where TMessage : class, IAPIEventMessage
        {
            var list = _handlers.GetOrAdd(typeof(TMessage), _ => new List<Delegate>());
            lock (list)
            {
                list.Add(handler);
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

        private void OnEventReceived(IAPIEventMessage message)
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
                    try
                    {
                        h.DynamicInvoke(message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception in realtime event handler for {MessageType}", messageType.Name);
                    }
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
}
