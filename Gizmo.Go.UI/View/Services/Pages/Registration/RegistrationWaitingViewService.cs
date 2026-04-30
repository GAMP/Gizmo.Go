using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationWaiting)]
    public sealed class RegistrationWaitingViewService : ViewStateServiceBase<RegistrationWaitingViewState>
    {
        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;
        private readonly IAppLifecycleService _appLifecycleService;
        private readonly IRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;

        private bool _isChecking;
        private CancellationTokenSource? _pollingCts;

        public RegistrationWaitingViewService(
            RegistrationWaitingViewState viewState,
            ILogger<RegistrationWaitingViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IAppLifecycleService appLifecycleService,
            IRegistrationService registrationService,
            IRegistrationSessionService registrationSession) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _appLifecycleService = appLifecycleService;
            _registrationService = registrationService;
            _registrationSession = registrationSession;
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters,
            CancellationToken cancellationToken = default)
        {
            if (!_registrationSession.HasToken)
            {
                _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
                return;
            }

            _appLifecycleService.Resumed += OnAppResumed;
            await _appLifecycleService.StartWatchingAsync();
            StartPolling();

            await base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters,
            CancellationToken cancellationToken = default)
        {
            _appLifecycleService.Resumed -= OnAppResumed;
            StopPolling();
            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        #endregion

        #region METHODS

        public ValueTask NavigateBackAsync()
        {
            StopPolling();
            _registrationSession.Clear();
            _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
            return ValueTask.CompletedTask;
        }

        #endregion

        #region PRIVATE METHODS

        private async void OnAppResumed(object? sender, EventArgs e)
        {
            await CheckConfirmationAsync();
        }

        private async Task CheckConfirmationAsync()
        {
            if (_isChecking)
                return;

            _isChecking = true;
            try
            {
                var result = await _registrationService.IsTokenConfirmedAsync(_registrationSession.Token);
                if (result.IsConfirmed)
                {
                    StopPolling();
                    if (!string.IsNullOrEmpty(result.Phone))
                        _registrationSession.SetPhone(result.Phone);
                    _navigationService.NavigateTo(NavigationHelper.RegistrationBotVerifySuccess);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Token confirmation check failed.");
                StopPolling();
                _registrationSession.Clear();
                _navigationService.NavigateTo(NavigationHelper.RegistrationProviders + "?error=1");
            }
            finally
            {
                _isChecking = false;
            }
        }

        private void StartPolling()
        {
            StopPolling();
            _pollingCts = new CancellationTokenSource();
            _ = PollAsync(_pollingCts.Token);
        }

        private async Task PollAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(5000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (!await _appLifecycleService.IsActiveAsync())
                    continue;

                await CheckConfirmationAsync();
            }
        }

        private void StopPolling()
        {
            _pollingCts?.Cancel();
            _pollingCts?.Dispose();
            _pollingCts = null;
        }

        #endregion
    }
}
