using Gizmo.Go.Core.Models.Events;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Helpers;
using Gizmo.Go.Provider.Direct.Mappers.Events;
using Gizmo.Web.Api.Messaging;
using MessagePack;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server user event hub (/api/user/events): a user connection gets its own balance and reservation signals and the news changes.
    /// </summary>
    internal sealed class DirectUserEventsService : IUserEventsService
    {
        private const string HubRoute = "api/user/events";
        private const string EventMethod = "Event";

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DirectUserEventsService> _logger;
        private readonly SemaphoreSlim _lock = new(1, 1);

        private HubConnection? _connection;
        private CancellationTokenSource? _connectCts;

        // the running first-connect loop; one at a time, a second loop would try to start a connection that is already started
        private Task _connectTask = Task.CompletedTask;

        public DirectUserEventsService(IServiceProvider serviceProvider, ILogger<DirectUserEventsService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public event EventHandler<UserEvent>? EventReceived;

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);

            try
            {
                if (_connection is not null)
                {
                    // a closed connection is started again; a live, reconnecting or still connecting one stays
                    if (_connection.State == HubConnectionState.Disconnected && _connectTask.IsCompleted && _connectCts is { IsCancellationRequested: false })
                        _connectTask = ConnectWithRetryAsync(_connection, _connectCts.Token);

                    return;
                }

                _connection = CreateConnection();
                _connectCts = new CancellationTokenSource();
                _connectTask = ConnectWithRetryAsync(_connection, _connectCts.Token);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);

            HubConnection? connection;

            try
            {
                connection = _connection;
                _connection = null;

                _connectCts?.Cancel();
                _connectCts = null;
            }
            finally
            {
                _lock.Release();
            }

            if (connection is null)
                return;

            try
            {
                await connection.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot close the user events connection.");
            }
        }

        private HubConnection CreateConnection()
        {
            var authService = _serviceProvider.GetRequiredService<IAuthService>();
            var url = BackendUrl.Resolve(_serviceProvider).TrimEnd('/') + "/" + HubRoute;

            var connection = new HubConnectionBuilder()
                .AddMessagePackProtocol(options =>
                {
                    options.SerializerOptions = MessagePackSerializerOptions.Standard
                        .WithResolver(MessagePack.Resolvers.StandardResolver.Instance)
                        .WithSecurity(MessagePackSecurity.TrustedData);
                })
                .WithAutomaticReconnect(new InfiniteRetryPolicy())
                .ConfigureLogging(logging =>
                {
#if DEBUG
                    // as the manager: the SignalR connection logs in debug builds
                    var loggerProvider = _serviceProvider.GetService<ILoggerProvider>();
                    if (loggerProvider != null)
                    {
                        logging.AddProvider(loggerProvider);
                        logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Information);
                        logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Information);
                    }
#endif
                })
                .WithUrl(url, options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                    options.AccessTokenProvider = () => authService.GetTokenAsync();
                })
                .Build();

            // the browsers throttle timers of background tabs, keep a generous window (the server timeout is 3 minutes)
            connection.ServerTimeout = TimeSpan.FromMinutes(2);

            connection.On<IUserEventMessage>(EventMethod, OnMessage);
            connection.Reconnecting += error =>
            {
                _logger.LogWarning(error, "User events connection lost, reconnecting.");
                return Task.CompletedTask;
            };

            return connection;
        }

        private void OnMessage(IUserEventMessage message)
        {
            try
            {
                // an unknown message (a newer server) is ignored
                if (UserEventMapper.Map(message) is { } userEvent)
                    EventReceived?.Invoke(this, userEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot handle the user event message.");
            }
        }

        // the first connect retries until the server answers; the automatic reconnect takes over after a connection was made.
        // Never throws: the task is not awaited.
        private async Task ConnectWithRetryAsync(HubConnection connection, CancellationToken cancellationToken)
        {
            var attempt = 0;

            // a connection that left the disconnected state (connected, reconnecting) is handled by the automatic reconnect
            while (!cancellationToken.IsCancellationRequested && connection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await connection.StartAsync(cancellationToken);
                    return;
                }
                catch (Exception ex)
                {
                    // stopped (the connection may be disposed already)
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    _logger.LogWarning(ex, "Cannot connect to the user events hub.");
                }

                try
                {
                    await Task.Delay(InfiniteRetryPolicy.GetDelay(attempt++), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        // reconnects forever with a growing pause (the user can be offline for a long time)
        private sealed class InfiniteRetryPolicy : IRetryPolicy
        {
            public TimeSpan? NextRetryDelay(RetryContext retryContext) => GetDelay((int)Math.Min(retryContext.PreviousRetryCount, 100));

            public static TimeSpan GetDelay(int attempt) => TimeSpan.FromSeconds(Math.Min(30, Math.Pow(2, Math.Min(attempt, 5))));
        }
    }
}
