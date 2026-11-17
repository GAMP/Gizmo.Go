using Gizmo.Go.Core.Models.Booking;
using Gizmo.Go.Core.Models.Branch;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Dialog;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages
{
    /// <summary>
    /// Booking of PCs in a club: session, PC selection and confirmation on one page (<see cref="NavigationHelper.BookingRoute"/>).
    /// The route has a parameter, which the navigation events of the view services do not match, so the page starts the
    /// load with the id from the route and applies the step of the query.
    /// </summary>
    [Register()]
    public sealed class BookingViewService : ViewStateServiceBase<BookingViewState>
    {
        private const int GroupColorCount = 6;
        private const int OtherTabColumns = 4;

        private static readonly int[] DurationChoices = [60, 120, 180, 240, 300, 360, 480];

        private readonly IBookingService _bookingService;
        private readonly IBranchProvider _branchProvider;
        private readonly IUserBalanceService _balanceService;
        private readonly IGizmoGoDialogService _dialogService;
        private readonly ILocalizationService _localizationService;
        private readonly NavigationService _navigationService;
        private readonly JSRuntimeService _jsRuntimeService;

        private CancellationTokenSource? _loadCts;
        private int _clubId;
        private string? _historyEntryState;

        public BookingViewService(
            BookingViewState viewState,
            ILogger<BookingViewService> logger,
            IServiceProvider serviceProvider,
            IBookingService bookingService,
            IBranchProvider branchProvider,
            IUserBalanceService balanceService,
            IGizmoGoDialogService dialogService,
            ILocalizationService localizationService,
            NavigationService navigationService,
            JSRuntimeService jsRuntimeService) : base(viewState, logger, serviceProvider)
        {
            _jsRuntimeService = jsRuntimeService;
            _bookingService = bookingService;
            _branchProvider = branchProvider;
            _balanceService = balanceService;
            _dialogService = dialogService;
            _localizationService = localizationService;
            _navigationService = navigationService;
        }

        private Branch? Club => ViewState.Club.Value?.Branch;

        private CancellationToken Token => (_loadCts ??= new CancellationTokenSource()).Token;

        /// <summary>
        /// Starts the booking of a club: clears the flow and loads the club and the reservation options.
        /// </summary>
        public async Task LoadAsync(int id)
        {
            CancelLoad();

            var cts = _loadCts = new CancellationTokenSource();
            _clubId = id;
            ViewState.Reset();
            RaiseViewStateChanged();

            await Task.WhenAll(
                ViewState.Club.LoadAsync(ct => _branchProvider.GetDetailsAsync(id, ct), Logger, "booking club", RaiseViewStateChanged, cts.Token),
                ViewState.Options.LoadAsync(ct => _bookingService.GetOptionsAsync(ct)!, Logger, "booking options", RaiseViewStateChanged, cts.Token));

            if (cts.IsCancellationRequested)
                return;

            ViewState.IsNotFound = !ViewState.Club.HasError && ViewState.Club.Value is null;

            if (Club is { } club && ViewState.Options.Value is { } options)
                Initialize(club, options);

            RaiseViewStateChanged();
        }

        /// <summary>
        /// Cancels the running requests (the page is left).
        /// </summary>
        public void CancelLoad()
        {
            _loadCts?.Cancel();
            _loadCts = null;
        }

        /// <summary>
        /// Shows the step of the query. A step whose data is missing goes back through the history when the entry was
        /// opened inside the flow (each forward step adds one entry after the previous step), otherwise (a reload of a
        /// direct entry, a link) it is replaced by step 1.
        /// </summary>
        /// <param name="historyEntryState">History entry state of the current screen.</param>
        public async Task ApplyStepAsync(int id, int step, string? historyEntryState)
        {
            _historyEntryState = historyEntryState;

            if (step is < 1 or > 3)
                step = 1;

            var isAvailable = step switch
            {
                2 => ViewState.HasSession && ViewState.HasAvailability,
                3 => ViewState.Offer is not null,
                _ => true
            };

            if (!isAvailable)
            {
                if (NavigationHelper.IsInAppEntry(historyEntryState))
                {
                    await _navigationService.GoBackAsync();
                    return;
                }

                step = 1;
                _navigationService.NavigateTo(NavigationHelper.GetBookingRoute(id), new NavigationOptions { ReplaceHistoryEntry = true });
                historyEntryState = null;
            }

            // the screen before the flow is known only for a step 1 opened inside the app (from the club page)
            if (step == 1)
                ViewState.IsFlowStartedInApp = NavigationHelper.IsInAppEntry(historyEntryState);

            if (ViewState.Step == step)
                return;

            ViewState.Step = step;
            RaiseViewStateChanged();
        }

        /// <summary>
        /// Returns to the previous step (the club page from step 1); a direct entry replaces itself with the previous screen.
        /// </summary>
        /// <param name="historyEntryState">History entry state of the current screen.</param>
        public Task BackAsync(string? historyEntryState)
        {
            var fallback = ViewState.Step <= 1
                ? NavigationHelper.GetClubRoute(_clubId)
                : NavigationHelper.GetBookingRoute(_clubId, ViewState.Step - 1);

            return _navigationService.GoBackOrNavigateAsync(historyEntryState, fallback);
        }

        /// <summary>
        /// Leaves the booking for the club page; no reservation is created. The flow entries are left behind in the
        /// history, so a back step from the club page does not return into the booking.
        /// </summary>
        public async Task CloseAsync()
        {
            var clubId = _clubId;
            var isStartedInApp = ViewState.IsFlowStartedInApp;
            var left = await _navigationService.LeaveFlowAsync(_jsRuntimeService, ViewState.FlowEntries);

            // stepped back to the club page itself; otherwise (a direct entry) the remaining entry becomes the club page
            if (!left || !isStartedInApp)
                _navigationService.NavigateTo(NavigationHelper.GetClubRoute(clubId), new NavigationOptions { ReplaceHistoryEntry = true });
        }

        /// <summary>
        /// Repeats the action of the current step that failed.
        /// </summary>
        public Task RetryAsync() => ViewState.Step switch
        {
            1 => SelectHostsAsync(),
            2 => ContinueAsync(),
            _ => BookAsync()
        };

        // the session and the selection do not change while a step action runs: its result belongs to the current values
        public void SelectDate(DateOnly date)
        {
            if (ViewState.IsBusy || ViewState.Date == date)
                return;

            ViewState.Date = date;

            // the chosen time may be in the past on another day (today)
            if (ViewState.StartTime is { } time && Club is { } club && !BookingTimeHelper.IsSlotSelectable(club, date, time))
            {
                ViewState.StartTime = null;
            }

            SessionChanged();
        }

        public void SelectStartTime(TimeOnly time)
        {
            if (ViewState.IsBusy)
                return;

            ViewState.StartTime = time;
            SessionChanged();
        }

        public void SelectDuration(int minutes)
        {
            if (ViewState.IsBusy)
                return;

            ViewState.Duration = minutes;
            SessionChanged();
        }

        /// <summary>
        /// Loads the halls, availability and prices of the session and opens the PC selection; without a free PC the
        /// "no options" dialog is shown.
        /// </summary>
        public async Task SelectHostsAsync()
        {
            if (ViewState.IsBusy || !ViewState.HasSession)
                return;

            var token = Token;
            ViewState.IsBusy = true;
            ViewState.HasActionError = false;
            RaiseViewStateChanged();

            var hasFreeHosts = false;

            try
            {
                if (!await LoadSelectionDataAsync(token))
                {
                    ViewState.IsNotFound = true;
                    return;
                }

                hasFreeHosts = ViewState.FreeHostIds.Count > 0;
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot load the booking availability.");
                ViewState.HasActionError = true;
            }
            finally
            {
                ViewState.IsBusy = false;
                RaiseViewStateChanged();
            }

            if (ViewState.HasActionError || ViewState.IsNotFound)
                return;

            if (!hasFreeHosts)
            {
                await ShowNoOptionsAsync();
                return;
            }

            _navigationService.NavigateTo(NavigationHelper.GetBookingRoute(_clubId, 2), NavigationHelper.InAppNavigation);
        }

        public void SelectTab(int key)
        {
            ViewState.SelectedTabKey = key;
            RaiseViewStateChanged();
        }

        public void SetMapView(bool isMap)
        {
            ViewState.IsMapView = isMap;
            RaiseViewStateChanged();
        }

        public void ZoomIn() => SetZoom(ViewState.Zoom + MapZoom.Step);

        public void ZoomOut() => SetZoom(ViewState.Zoom - MapZoom.Step);

        /// <summary>
        /// Fits the map of the selected tab to the screen width.
        /// </summary>
        public void ZoomFit()
        {
            var columns = ViewState.Tabs.FirstOrDefault(tab => tab.Key == ViewState.SelectedTabKey)?.Hosts
                .Select(host => host.Column + 1)
                .DefaultIfEmpty(1)
                .Max() ?? 1;

            SetZoom(MapZoom.GetFit(columns));
        }

        public void ToggleHost(int hostId)
        {
            if (ViewState.IsBusy || !ViewState.FreeHostIds.Contains(hostId))
                return;

            var selected = ViewState.SelectedHostIds.ToList();

            if (!selected.Remove(hostId))
            {
                // one host per reservation: the new host replaces the previous one
                if (ViewState.Options.Value?.MultiHostDisable == true)
                    selected.Clear();

                selected.Add(hostId);
            }

            ViewState.SelectedHostIds = selected;
            ViewState.Offer = null;

            // the next selection opens the panel expanded again
            if (selected.Count == 0)
                ViewState.IsSummaryExpanded = true;

            RaiseViewStateChanged();
        }

        public void ClearHosts()
        {
            if (ViewState.IsBusy)
                return;

            ViewState.SelectedHostIds = [];
            ViewState.IsSummaryExpanded = true;
            ViewState.Offer = null;
            RaiseViewStateChanged();
        }

        public void ToggleSummary()
        {
            ViewState.IsSummaryExpanded = !ViewState.IsSummaryExpanded;
            RaiseViewStateChanged();
        }

        /// <summary>
        /// Creates the offer for the selected hosts and opens the confirmation. A host that is no longer free or has no
        /// price shows the "host already taken" sheet.
        /// </summary>
        public async Task ContinueAsync()
        {
            if (ViewState.IsBusy || ViewState.SelectedHostIds.Count == 0 || !TryGetStartUtc(out var startUtc))
                return;

            var token = Token;
            ViewState.IsBusy = true;
            ViewState.HasActionError = false;
            RaiseViewStateChanged();

            BookingOfferResult? result = null;

            try
            {
                result = await _bookingService.CreateOfferAsync(_clubId, startUtc, ViewState.Duration!.Value, ViewState.SelectedHostIds, token);

                if (result.Code == BookingOfferResultCode.Success && result.Offer is not null)
                {
                    ViewState.Offer = result.Offer;
                    ViewState.AmountKind = BookingAmountKind.Full;
                    await LoadBalanceAsync(token);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot create the booking offer.");
                ViewState.HasActionError = true;
            }
            finally
            {
                ViewState.IsBusy = false;
                RaiseViewStateChanged();
            }

            if (result is null || ViewState.HasActionError)
                return;

            if (result.Code == BookingOfferResultCode.Success && result.Offer is not null)
                _navigationService.NavigateTo(NavigationHelper.GetBookingRoute(_clubId, 3), NavigationHelper.InAppNavigation);
            else
                await HandleHostsUnavailableAsync(result.UnavailableHostIds);
        }

        public void SelectAmountKind(BookingAmountKind kind)
        {
            ViewState.AmountKind = kind;
            RaiseViewStateChanged();
        }

        public Task RetryBalanceAsync() => LoadBalanceAsync(Token);

        public async Task ShowBookingRulesAsync()
        {
            try
            {
                await _dialogService.ShowRulesDialogAsync(_localizationService.GetString(nameof(Autogenerated.Resources.GO_CLUB_RULES_BOOKING)));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot show the booking rules dialog.");
            }
        }

        /// <summary>
        /// Books the offer: pays exactly the chosen amount from the deposit and creates the reservation.
        /// </summary>
        public async Task BookAsync()
        {
            if (ViewState.IsBusy || ViewState.Offer is not { } offer || ViewState.PaymentAmount is not { } amount)
                return;

            var token = Token;
            ViewState.IsBusy = true;
            ViewState.HasActionError = false;
            RaiseViewStateChanged();

            BookingResult? result = null;
            BookingOfferResult? renewed = null;

            try
            {
                result = await _bookingService.BookAsync(offer.OfferId, amount, token);

                switch (result.Code)
                {
                    case BookingResultCode.InsufficientDeposit:
                        await LoadBalanceAsync(token);
                        break;

                    case BookingResultCode.OfferExpired:
                        renewed = await RenewOfferAsync(offer, token);
                        break;
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot book the offer.");
                ViewState.HasActionError = true;
            }
            finally
            {
                ViewState.IsBusy = false;
                RaiseViewStateChanged();
            }

            if (result is null || ViewState.HasActionError)
                return;

            switch (result.Code)
            {
                case BookingResultCode.Success:
                    await ShowSuccessAsync(offer, result.ReservationId);
                    break;

                // the booking is refused: the sheet leads back to the PC selection or to the session, as at step 2
                case BookingResultCode.HostsUnavailable:
                    await HandleHostsUnavailableAsync(result.UnavailableHostIds);
                    break;

                // the selection cannot be offered again
                case BookingResultCode.OfferExpired when renewed is { Code: not BookingOfferResultCode.Success }:
                    await HandleHostsUnavailableAsync(renewed.UnavailableHostIds);
                    break;
            }
        }

        private void Initialize(Branch club, BookingOptions options)
        {
            ViewState.Durations = DurationChoices
                .Where(minutes => (options.MinimumTime is not { } minimum || minutes >= minimum)
                                  && (options.MaximumTime is not { } maximum || minutes <= maximum))
                .ToList();

            ViewState.Date = BookingTimeHelper.GetFirstSelectableDay(club) ?? BookingTimeHelper.GetClubToday(club);
        }

        // changing the session invalidates everything the PC selection loaded for the previous one
        private void SessionChanged()
        {
            ViewState.ResetSelection();
            ViewState.HasActionError = false;
            RaiseViewStateChanged();
        }

        private void SetZoom(int zoom)
        {
            ViewState.Zoom = Math.Clamp(zoom, MapZoom.Min, MapZoom.Max);
            RaiseViewStateChanged();
        }

        private bool TryGetStartUtc(out DateTime startUtc)
        {
            startUtc = default;

            return Club is { } club
                   && ViewState.Date is { } date
                   && ViewState.StartTime is { } time
                   && BookingTimeHelper.TryGetStartUtc(club, date, time, out startUtc);
        }

        // false when the club does not exist
        private async Task<bool> LoadSelectionDataAsync(CancellationToken token)
        {
            if (!TryGetStartUtc(out var startUtc))
                return true;

            var duration = ViewState.Duration!.Value;

            var hallsTask = ViewState.Halls is { } loaded
                ? Task.FromResult<BookingHalls?>(loaded)
                : _bookingService.GetHallsAsync(_clubId, token);
            var availabilityTask = _bookingService.GetAvailabilityAsync(_clubId, startUtc, duration, token);
            var pricesTask = _bookingService.GetPricesAsync(_clubId, startUtc, duration, token);

            await Task.WhenAll(hallsTask, availabilityTask, pricesTask);
            token.ThrowIfCancellationRequested();

            if (hallsTask.Result is not { } halls)
                return false;

            var prices = new Dictionary<int, BookingHostGroupPrice>();
            foreach (var price in pricesTask.Result)
                prices[price.HostGroupId] = price;

            var available = availabilityTask.Result.Where(item => item.IsAvailable).Select(item => item.HostId).ToHashSet();

            // a host of a host group without a price for the session cannot be booked, it counts as busy
            var free = halls.Hosts
                .Where(host => available.Contains(host.Id) && prices.TryGetValue(host.HostGroupId, out var price) && price.Price is not null)
                .Select(host => host.Id)
                .ToHashSet();

            ViewState.Halls = halls;
            ViewState.Prices = prices;
            ViewState.FreeHostIds = free;
            ViewState.GroupColors = halls.HostGroups
                .OrderBy(group => group.Name, StringComparer.CurrentCulture)
                .Select((group, index) => (group.Id, Index: index % GroupColorCount))
                .ToDictionary(item => item.Id, item => item.Index);
            ViewState.Tabs = BuildTabs(halls);
            ViewState.SelectedHostIds = ViewState.SelectedHostIds.Where(free.Contains).ToList();
            ViewState.Offer = null;
            ViewState.HasAvailability = true;

            if (ViewState.Tabs.All(tab => tab.Key != ViewState.SelectedTabKey))
            {
                ViewState.SelectedTabKey = (ViewState.Tabs.FirstOrDefault(tab => tab.Hosts.Any(host => free.Contains(host.Host.Id)))
                                            ?? ViewState.Tabs.FirstOrDefault())?.Key ?? 0;
            }

            return true;
        }

        private IReadOnlyList<BookingTab> BuildTabs(BookingHalls halls)
        {
            var hostsById = halls.Hosts.ToDictionary(host => host.Id);
            var tabs = new List<BookingTab>();
            var placed = new HashSet<int>();

            foreach (var hall in halls.Halls.OrderBy(hall => hall.DisplayOrder))
            {
                var entries = hall.Hosts.Where(entry => hostsById.ContainsKey(entry.HostId)).ToList();
                if (entries.Count == 0)
                    continue;

                var minRow = entries.Min(entry => entry.Row);
                var minColumn = entries.Min(entry => entry.Column);

                tabs.Add(new BookingTab
                {
                    Key = hall.Id,
                    Name = hall.Name,
                    Hosts = entries
                        .Select(entry => new BookingTabHost { Host = hostsById[entry.HostId], Row = entry.Row - minRow, Column = entry.Column - minColumn })
                        .ToList()
                });

                foreach (var entry in entries)
                    placed.Add(entry.HostId);
            }

            var others = halls.Hosts.Where(host => !placed.Contains(host.Id)).OrderBy(host => host.Number).ToList();

            if (others.Count > 0)
            {
                var isOnlyTab = tabs.Count == 0;

                tabs.Add(new BookingTab
                {
                    Key = isOnlyTab ? BookingTab.AllKey : BookingTab.OtherKey,
                    Name = _localizationService.GetString(isOnlyTab
                        ? nameof(Autogenerated.Resources.GO_BOOKING_TAB_ALL)
                        : nameof(Autogenerated.Resources.GO_BOOKING_TAB_OTHER)),
                    Hosts = others
                        .Select((host, index) => new BookingTabHost { Host = host, Row = index / OtherTabColumns, Column = index % OtherTabColumns })
                        .ToList()
                });
            }

            return tabs;
        }

        private Task LoadBalanceAsync(CancellationToken token) =>
            ViewState.Balance.LoadAsync(ct => _balanceService.GetAsync(ct)!, Logger, "booking balance", RaiseViewStateChanged, token);

        // the offer cart lives on the server for a limited time: a new offer for the same hosts keeps the user on the step;
        // a result other than success is returned for the "host already taken" sheet, null when no offer was requested
        private async Task<BookingOfferResult?> RenewOfferAsync(BookingOffer offer, CancellationToken token)
        {
            if (!TryGetStartUtc(out var startUtc))
            {
                ViewState.HasActionError = true;
                return null;
            }

            var hostIds = offer.Hosts.Select(host => host.HostId).ToList();
            var result = await _bookingService.CreateOfferAsync(_clubId, startUtc, ViewState.Duration!.Value, hostIds, token);

            if (result.Code == BookingOfferResultCode.Success && result.Offer is null)
            {
                ViewState.HasActionError = true;
                return null;
            }

            if (result.Offer is { } renewed && result.Code == BookingOfferResultCode.Success)
            {
                ViewState.Offer = renewed;

                if (renewed.MinimumPayment >= renewed.Total)
                    ViewState.AmountKind = BookingAmountKind.Full;
            }

            return result;
        }

        private async Task ShowNoOptionsAsync()
        {
            try
            {
                var dialog = await _dialogService.ShowBookingNoOptionsDialogAsync();
                if (dialog.Result != AddComponentResultCode.Opened)
                    return;

                // the time grid stays on the screen, "choose another time" only closes the dialog
                var result = await dialog.WaitForResultAsync();
                if (dialog.Result != AddComponentResultCode.Ok || result?.LeaveBooking != true)
                    return;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot show the booking no options dialog.");
                return;
            }

            // "close" leaves the booking for Home, as a finished booking does
            await FinishFlowAsync(null);
        }

        // the hosts are marked busy; the sheet leads back to the PC selection (fresh availability) or to the session
        private async Task HandleHostsUnavailableAsync(IReadOnlyCollection<int> hostIds)
        {
            try
            {
                var taken = hostIds.ToHashSet();
                ViewState.FreeHostIds = ViewState.FreeHostIds.Where(id => !taken.Contains(id)).ToHashSet();
                ViewState.SelectedHostIds = ViewState.SelectedHostIds.Where(id => !taken.Contains(id)).ToList();
                RaiseViewStateChanged();

                var dialog = await _dialogService.ShowBookingHostTakenDialogAsync(BuildTakenItems(taken));
                var changeTime = false;

                if (dialog.Result == AddComponentResultCode.Opened)
                {
                    var result = await dialog.WaitForResultAsync();
                    changeTime = dialog.Result == AddComponentResultCode.Ok && result?.ChangeTime == true;
                }

                ViewState.Offer = null;

                if (changeTime)
                {
                    ViewState.ResetSelection();
                    RaiseViewStateChanged();

                    // back through the history: from step 3 the step 2 entry has no data any more and steps back itself
                    await _navigationService.GoBackOrNavigateAsync(_historyEntryState, NavigationHelper.GetBookingRoute(_clubId));
                    return;
                }

                await RefreshSelectionAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot handle the hosts that are no longer available.");
            }
        }

        // reloads availability and prices and shows the PC selection
        private async Task RefreshSelectionAsync()
        {
            var token = Token;
            ViewState.IsBusy = true;
            ViewState.HasActionError = false;
            RaiseViewStateChanged();

            try
            {
                await LoadSelectionDataAsync(token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot reload the booking availability.");
                ViewState.HasActionError = true;
            }
            finally
            {
                ViewState.IsBusy = false;
                RaiseViewStateChanged();
            }

            // step 3 was opened from step 2: back through the history, so no duplicate step 2 entry is left
            if (ViewState.Step == 3)
                await _navigationService.GoBackOrNavigateAsync(_historyEntryState, NavigationHelper.GetBookingRoute(_clubId, 2));
        }

        private List<BookingHostTakenItem> BuildTakenItems(IReadOnlySet<int> hostIds)
        {
            var halls = ViewState.Halls;
            if (halls is null)
                return [];

            return halls.Hosts
                .Where(host => hostIds.Contains(host.Id))
                .OrderBy(host => host.Number)
                .Select(host =>
                {
                    var groupName = halls.HostGroups.FirstOrDefault(group => group.Id == host.HostGroupId)?.Name ?? string.Empty;
                    var rate = ViewState.Prices.TryGetValue(host.HostGroupId, out var price)
                        ? BookingTimeHelper.FormatRate(price.HourlyRate, _localizationService)
                        : string.Empty;

                    return new BookingHostTakenItem
                    {
                        Number = host.Number,
                        Name = host.Name,
                        Details = string.Join(" · ", new[] { groupName, rate }.Where(text => text.Length > 0)),
                        GroupColor = ViewState.GetGroupColor(host.HostGroupId)
                    };
                })
                .ToList();
        }

        private async Task ShowSuccessAsync(BookingOffer offer, int? reservationId)
        {
            try
            {
                // the booked hosts are the hosts of the offer
                var numbers = offer.Hosts
                    .Select(item => ViewState.GetHost(item.HostId))
                    .OfType<BookingHost>()
                    .OrderBy(host => host.Number)
                    .Select(host => BookingTimeHelper.FormatHost(host.Number, _localizationService));

                var date = ViewState.Date!.Value;
                var start = ViewState.StartTime!.Value;
                var end = start.AddMinutes(ViewState.Duration!.Value);
                var period = $"{BookingTimeHelper.FormatDate(date)} · {BookingTimeHelper.FormatTime(start)}–{BookingTimeHelper.FormatTime(end)}";

                var dialog = await _dialogService.ShowBookingSuccessDialogAsync(
                    string.Join(", ", numbers),
                    period,
                    ReservationDisplayHelper.FormatAmount(offer.Total));

                var result = dialog.Result == AddComponentResultCode.Opened ? await dialog.WaitForResultAsync() : null;

                await FinishFlowAsync(result?.OpenReservation == true ? reservationId : null);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot show the booking success dialog.");
                await FinishFlowAsync(null);
            }
        }

        // a finished booking opens Home with the flow removed from the back steps (as a finished registration): the
        // history steps back over the flow, the screen it was opened from is replaced by Home, the flow state is cleared;
        // "My reservations" opens the new reservation over Home
        private async Task FinishFlowAsync(int? reservationId)
        {
            await _navigationService.LeaveFlowAsync(_jsRuntimeService, ViewState.FlowEntries);
            _navigationService.NavigateTo(NavigationHelper.HomePage, new NavigationOptions { ReplaceHistoryEntry = true });

            if (reservationId is { } id)
                _navigationService.NavigateTo(NavigationHelper.GetReservationRoute(id), NavigationHelper.InAppNavigation);

            // the booking page is gone, nothing of the finished flow stays in memory
            CancelLoad();
            ViewState.Reset();
        }

    }

    /// <summary>
    /// Zoom steps of the host map, in percent.
    /// </summary>
    public static class MapZoom
    {
        /// <summary>Gets the zoom step.</summary>
        public const int Step = 25;

        /// <summary>Gets the minimum zoom.</summary>
        public const int Min = 50;

        /// <summary>Gets the maximum zoom.</summary>
        public const int Max = 200;

        // width of the map grid on a phone (the map without its paddings) and of a tile with its gap at 100 %, in px
        private const int AreaWidth = 263;
        private const int CellWidth = 66;

        /// <summary>
        /// Gets the biggest zoom at which a map of <paramref name="columns"/> columns fits the area width.
        /// </summary>
        public static int GetFit(int columns)
        {
            var zoom = AreaWidth * 100 / (Math.Max(columns, 1) * CellWidth);
            return Math.Clamp(zoom / Step * Step, Min, Max);
        }
    }
}
