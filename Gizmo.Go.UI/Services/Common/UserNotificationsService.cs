using Gizmo.Go.Core.Models.Events;
using Gizmo.Go.Core.Models.News;
using Gizmo.Go.Core.Models.Notification;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Core.Services.Notification;
using Gizmo.Go.UI.Helpers;
using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.Services.Common
{
    /// <inheritdoc cref="IUserNotificationsService"/>
    public sealed class UserNotificationsService : IUserNotificationsService
    {
        private readonly IAuthService _authService;
        private readonly IAppLifecycleService _appLifecycleService;
        private readonly IUserEventsService _userEventsService;
        private readonly IUserBalanceService _userBalanceService;
        private readonly INewsService _newsService;
        private readonly IDeviceSettingsService _deviceSettingsService;
        private readonly IGizmoGoNotificationsService _notificationsService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger<UserNotificationsService> _logger;

        // the last known deposits of the session (the balance at the start, then the balance events): balance events also
        // come with every usage charge, so a toast is shown only when the deposits grew
        private readonly object _depositsLock = new();
        private decimal? _lastDeposits;

        // one balance read at a time: parallel reads may answer out of order and give two toasts for one top up
        private readonly SemaphoreSlim _topUpCheckLock = new(1, 1);

        // the news ids the user could see last time: a news change event carries no news, the list is compared instead.
        // One save gives several events (the news and its branches), so the check runs once after a short delay
        private const int NEWS_CHECK_DELAY_MS = 1000;
        private readonly object _newsLock = new();
        private HashSet<int>? _knownNewsIds;
        private bool _isNewsCheckPending;

        // changes on every stop, so a baseline loaded for a previous session is not applied
        private int _session;
        private bool _isRunning;

        public UserNotificationsService(
            IAuthService authService,
            IAppLifecycleService appLifecycleService,
            IUserEventsService userEventsService,
            IUserBalanceService userBalanceService,
            INewsService newsService,
            IDeviceSettingsService deviceSettingsService,
            IGizmoGoNotificationsService notificationsService,
            ILocalizationService localizationService,
            ILogger<UserNotificationsService> logger)
        {
            _authService = authService;
            _appLifecycleService = appLifecycleService;
            _userEventsService = userEventsService;
            _userBalanceService = userBalanceService;
            _newsService = newsService;
            _deviceSettingsService = deviceSettingsService;
            _notificationsService = notificationsService;
            _localizationService = localizationService;
            _logger = logger;
        }

        public void Run()
        {
            if (_isRunning)
                return;

            _isRunning = true;

            _authService.StateChanged += OnAuthStateChanged;
            _appLifecycleService.Resumed += OnResumed;
            _userEventsService.EventReceived += OnEventReceived;

            if (_authService.State == AuthState.Authenticated)
                _ = StartAsync();
        }

        private void OnAuthStateChanged(object? sender, AuthState state)
        {
            if (state == AuthState.Authenticated)
                _ = StartAsync();
            else if (state == AuthState.Unauthenticated)
                _ = StopAsync();
        }

        // the connection may have dropped while the app was in the background
        private void OnResumed(object? sender, EventArgs e)
        {
            if (_authService.State == AuthState.Authenticated)
                _ = StartAsync();
        }

        private async Task StartAsync()
        {
            try
            {
                await _userEventsService.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot start the user events.");
            }

            await TrySetDepositsBaselineAsync();
            await TrySetNewsBaselineAsync();
        }

        private async Task StopAsync()
        {
            try
            {
                lock (_depositsLock)
                {
                    _session++;
                    _lastDeposits = null;
                }

                lock (_newsLock)
                    _knownNewsIds = null;

                await _userEventsService.StopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot stop the user events.");
            }
        }

        // the first top up after the start is compared with the current balance; without it (server not reachable) the first
        // balance event only sets the baseline
        private async Task TrySetDepositsBaselineAsync()
        {
            int session;

            lock (_depositsLock)
            {
                if (_lastDeposits.HasValue)
                    return;

                session = _session;
            }

            try
            {
                var balance = await _userBalanceService.GetAsync();

                lock (_depositsLock)
                {
                    // a balance event that came first is newer than this value
                    if (session == _session && !_lastDeposits.HasValue)
                        _lastDeposits = balance.Deposits;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot load the balance for the top up notifications.");
            }
        }

        // reads the deposits and keeps them as the known value; true when they grew. A failed read keeps the known value
        private async Task<bool> IsTopUpAsync()
        {
            await _topUpCheckLock.WaitAsync();

            try
            {
                int session;

                lock (_depositsLock)
                    session = _session;

                var balance = await _userBalanceService.GetAsync();

                lock (_depositsLock)
                {
                    // the balance of a previous session is not applied
                    if (session != _session)
                        return false;

                    var isTopUp = _lastDeposits.HasValue && balance.Deposits > _lastDeposits.Value;
                    _lastDeposits = balance.Deposits;
                    return isTopUp;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot load the balance for the top up notification.");
                return false;
            }
            finally
            {
                _topUpCheckLock.Release();
            }
        }

        private async void OnEventReceived(object? sender, UserEvent userEvent)
        {
            try
            {
                // the balance event is a signal: the deposits are read, then compared with the known value
                var isTopUp = userEvent is UserBalanceChangedEvent && await IsTopUpAsync();

                var settings = await _deviceSettingsService.GetAsync();
                switch (userEvent)
                {
                    case UserReservationEvent reservation when settings.PersonalNotifications:
                        await ShowAsync(_localizationService.GetString(reservation.Kind == UserReservationEventKind.Created
                                ? nameof(Autogenerated.Resources.GO_NOTIFY_RESERVATION_CREATED)
                                : nameof(Autogenerated.Resources.GO_NOTIFY_RESERVATION_CANCELED), reservation.ReservationId),
                            NavigationHelper.GetReservationsRoute(reservation.ReservationId));
                        break;

                    case UserBalanceChangedEvent when isTopUp && settings.PersonalNotifications:
                        await ShowAsync(_localizationService.GetString(nameof(Autogenerated.Resources.GO_NOTIFY_BALANCE_TOPUP)), NavigationHelper.HomePage);
                        break;

                    case NewsChangedEvent:
                        await CheckNewsAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot show the user event notification.");
            }
        }

        private async Task TrySetNewsBaselineAsync()
        {
            lock (_newsLock)
            {
                if (_knownNewsIds != null)
                    return;
            }

            try
            {
                var news = await _newsService.GetListAsync();

                lock (_newsLock)
                    _knownNewsIds ??= news.Select(item => item.Id).ToHashSet();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot load the news for the news notifications.");
            }
        }

        // a news item not in the known list became visible; without a known list (server not reachable at the start) the
        // first check only sets it
        private async Task CheckNewsAsync()
        {
            lock (_newsLock)
            {
                if (_isNewsCheckPending)
                    return;

                _isNewsCheckPending = true;
            }

            IReadOnlyList<NewsItem> news;

            try
            {
                await Task.Delay(NEWS_CHECK_DELAY_MS);
                news = await _newsService.GetListAsync();
            }
            finally
            {
                lock (_newsLock)
                    _isNewsCheckPending = false;
            }

            List<NewsItem> published;

            lock (_newsLock)
            {
                published = _knownNewsIds == null ? [] : news.Where(item => !_knownNewsIds.Contains(item.Id)).ToList();
                _knownNewsIds = news.Select(item => item.Id).ToHashSet();
            }

            if (published.Count == 0 || !(await _deviceSettingsService.GetAsync()).NewsNotifications)
                return;

            foreach (var item in published)
                await ShowAsync(_localizationService.GetString(nameof(Autogenerated.Resources.GO_NOTIFY_NEWS), item.Title), NavigationHelper.GetNewsDetailsRoute(item.Id));
        }

        private Task ShowAsync(string message, string route) => _notificationsService.ShowAsync(message, GoNotificationType.Info, route);
    }
}
