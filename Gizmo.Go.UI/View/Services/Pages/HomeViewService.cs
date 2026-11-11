using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Models.Events;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Common;
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
    /// Home screen. Every block loads in parallel and fails on its own. A return to the screen refreshes the blocks and keeps
    /// the shown data and the selected city until the new data arrives; a logout clears them.
    /// </summary>
    [Register()]
    [Route(NavigationHelper.HomePage)]
    public sealed class HomeViewService : ViewStateServiceBase<HomeViewState>
    {
        private const int CarouselNewsCount = 5;

        private readonly IAuthService _authService;
        private readonly IAuthOptionsService _authOptionsService;
        private readonly ILadderService _ladderService;
        private readonly IUserBalanceService _userBalanceService;
        private readonly IUserEventsService _userEventsService;
        private readonly IReservationService _reservationService;
        private readonly INewsService _newsService;
        private readonly IBranchProvider _branchProvider;
        private readonly IGizmoGoDialogService _dialogService;
        private readonly NavigationService _navigationService;

        private CancellationTokenSource? _loadCts;
        private CancellationTokenSource? _clubsCts;

        public HomeViewService(
            HomeViewState viewState,
            ILogger<HomeViewService> logger,
            IServiceProvider serviceProvider,
            IAuthService authService,
            IAuthOptionsService authOptionsService,
            ILadderService ladderService,
            IUserBalanceService userBalanceService,
            IUserEventsService userEventsService,
            IReservationService reservationService,
            INewsService newsService,
            IBranchProvider branchProvider,
            IGizmoGoDialogService dialogService,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _authService = authService;
            _authOptionsService = authOptionsService;
            _ladderService = ladderService;
            _userBalanceService = userBalanceService;
            _userEventsService = userEventsService;
            _reservationService = reservationService;
            _newsService = newsService;
            _branchProvider = branchProvider;
            _dialogService = dialogService;
            _navigationService = navigationService;
        }

        /// <summary>
        /// Loads the blocks when the screen was opened without a navigation event (the first screen of the app start).
        /// </summary>
        public Task EnsureLoadedAsync() => _loadCts is null ? LoadAsync() : Task.CompletedTask;

        public async Task SelectCityAsync(string city)
        {
            if (string.Equals(ViewState.SelectedCity, city, StringComparison.Ordinal))
                return;

            ViewState.SelectedCity = city;

            // the clubs of another city: the old list is not shown while the new one loads
            ViewState.Clubs.Reset();
            await LoadClubsAsync(GetLoadToken());
        }

        /// <summary>
        /// Reloads the clubs of the selected city.
        /// </summary>
        public Task ReloadClubsAsync() => LoadClubsAsync(GetLoadToken());

        /// <summary>
        /// Reloads the cities and the clubs.
        /// </summary>
        public Task ReloadCitiesAsync() => LoadCitiesAsync(GetLoadToken());

        public async Task OpenLadderInfoAsync()
        {
            try
            {
                await _dialogService.ShowLadderInfoDialogAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot show the ladder info dialog.");
            }
        }

        public void OpenProfile() => _navigationService.NavigateTo(NavigationHelper.ProfileRoute, NavigationHelper.InAppNavigation);

        public void OpenAllNews() => _navigationService.NavigateTo(NavigationHelper.NewsRoute, NavigationHelper.InAppNavigation);

        public void OpenReservations() => _navigationService.NavigateTo(NavigationHelper.ReservationsRoute, NavigationHelper.InAppNavigation);

        public void OpenNews(int id) => _navigationService.NavigateTo(NavigationHelper.GetNewsDetailsRoute(id), NavigationHelper.InAppNavigation);

        protected override Task OnInitializing(CancellationToken ct)
        {
            _authService.StateChanged += OnAuthStateChanged;
            _userEventsService.EventReceived += OnUserEventReceived;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _authService.StateChanged -= OnAuthStateChanged;
            _userEventsService.EventReceived -= OnUserEventReceived;
            base.OnDisposing(isDisposing);
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            // an unauthenticated user is redirected to Welcome by the page, the secured calls would only fail
            if (_authService.State != AuthState.Authenticated)
                return Task.CompletedTask;

            return LoadAsync();
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            CancelLoad();
            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        private void OnAuthStateChanged(object? sender, AuthState state)
        {
            if (state != AuthState.Unauthenticated)
                return;

            // the data of the previous user is never shown to the next one
            CancelLoad();
            ResetBlocks();
        }

        // the balance and news blocks follow the user events while the screen is open (top ups, usage charges, news changes);
        // the old value stays on screen until the new one arrives
        private void OnUserEventReceived(object? sender, UserEvent userEvent)
        {
            var loadCts = _loadCts;
            if (loadCts is null)
                return;

            switch (userEvent)
            {
                case UserBalanceChangedEvent:
                    _ = ViewState.Balance.LoadAsync(ct => _userBalanceService.GetAsync(ct)!, Logger, "balance", DebounceViewStateChanged, loadCts.Token);
                    break;

                case NewsChangedEvent:
                    _ = ViewState.News.LoadAsync(ct => _newsService.GetListAsync(CarouselNewsCount, ct)!, Logger, "news", DebounceViewStateChanged, loadCts.Token);
                    break;
            }
        }

        private CancellationToken GetLoadToken() => (_loadCts ??= new CancellationTokenSource()).Token;

        private void CancelLoad()
        {
            _clubsCts?.Cancel();
            _clubsCts = null;
            _loadCts?.Cancel();
            _loadCts = null;
        }

        private void ResetBlocks()
        {
            ViewState.Logo.Reset();
            ViewState.Level.Reset();
            ViewState.Balance.Reset();
            ViewState.Reservation.Reset();
            ViewState.News.Reset();
            ViewState.Cities.Reset();
            ViewState.Clubs.Reset();
            ViewState.SelectedCity = null;
        }

        private async Task LoadAsync()
        {
            CancelLoad();

            _loadCts = new CancellationTokenSource();
            var cancellationToken = _loadCts.Token;

            RaiseViewStateChanged();

            await Task.WhenAll(
                ViewState.Logo.LoadAsync(async ct => (await _authOptionsService.GetAsync(ct))?.LogoUrl, Logger, "logo", DebounceViewStateChanged, cancellationToken),
                ViewState.Level.LoadAsync(ct => _ladderService.GetLevelAsync(ct), Logger, "ladder level", DebounceViewStateChanged, cancellationToken),
                ViewState.Balance.LoadAsync(ct => _userBalanceService.GetAsync(ct)!, Logger, "balance", DebounceViewStateChanged, cancellationToken),
                ViewState.Reservation.LoadAsync(async ct => (await _reservationService.GetCurrentAsync(ct)).FirstOrDefault(), Logger, "nearest reservation", DebounceViewStateChanged, cancellationToken),
                ViewState.News.LoadAsync(ct => _newsService.GetListAsync(CarouselNewsCount, ct)!, Logger, "news", DebounceViewStateChanged, cancellationToken),
                LoadCitiesAsync(cancellationToken));

            if (!cancellationToken.IsCancellationRequested)
                RaiseViewStateChanged();
        }

        private async Task LoadCitiesAsync(CancellationToken cancellationToken)
        {
            await ViewState.Cities.LoadAsync(ct => _branchProvider.GetCitiesAsync(ct)!, Logger, "cities", DebounceViewStateChanged, cancellationToken);

            if (cancellationToken.IsCancellationRequested || ViewState.Cities.HasError)
                return;

            // the first city is the default one; the selected city is kept while it exists
            var cities = ViewState.Cities.Value;

            if (cities is not { Count: > 0 })
            {
                ViewState.SelectedCity = null;
                ViewState.Clubs.SetValue([]);
                DebounceViewStateChanged();
                return;
            }

            if (ViewState.SelectedCity is null || !cities.Contains(ViewState.SelectedCity))
            {
                ViewState.SelectedCity = cities[0];
                ViewState.Clubs.Reset();
            }

            await LoadClubsAsync(cancellationToken);
        }

        private async Task LoadClubsAsync(CancellationToken loadToken)
        {
            _clubsCts?.Cancel();
            _clubsCts = CancellationTokenSource.CreateLinkedTokenSource(loadToken);
            var cancellationToken = _clubsCts.Token;

            var filter = new BranchFilter { City = ViewState.SelectedCity };

            await ViewState.Clubs.LoadAsync(async ct => (await _branchProvider.GetAsync(filter, ct)).ToList(), Logger, "clubs", DebounceViewStateChanged, cancellationToken);
        }
    }
}
