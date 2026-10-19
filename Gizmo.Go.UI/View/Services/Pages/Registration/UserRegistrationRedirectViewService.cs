using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationRedirectRoute)]
    public sealed class UserRegistrationRedirectViewService : ViewStateServiceBase<UserRegistrationRedirectViewState>
    {
        private static readonly TimeSpan QrExpiryDelay = TimeSpan.FromMinutes(4);
        private static readonly TimeSpan TokenPollInterval = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan TransientFailureRetryDelay = TimeSpan.FromSeconds(1);
        private const int MaxTransientFailureRetries = 1;

        public UserRegistrationRedirectViewService(
            UserRegistrationRedirectViewState viewState,
            ILogger<UserRegistrationRedirectViewService> logger,
            IServiceProvider serviceProvider,
            IUserRegistrationService registrationService,
            IRegistrationSessionService registrationSession,
            IExternalLauncher externalLauncher,
            IAppLifecycleService appLifecycleService) : base(viewState, logger, serviceProvider)
        {
            _registrationService = registrationService;
            _registrationSession = registrationSession;
            _externalLauncher = externalLauncher;
            _appLifecycleService = appLifecycleService;
        }

        private readonly IUserRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly IExternalLauncher _externalLauncher;
        private readonly IAppLifecycleService _appLifecycleService;
        private CancellationTokenSource? _qrExpiryCts;
        private bool _isChecking;

        public Task NavigateBackAsync()
        {
            CancelQrExpiry();
            _registrationSession.Clear();
            NavigationService.NavigateTo(NavigationHelper.RegistrationProvidersRoute);
            return Task.CompletedTask;
        }

        public Task NavigateToProvidersAsync()
        {
            CancelQrExpiry();
            _registrationSession.Clear();
            NavigationService.NavigateTo(NavigationHelper.RegistrationProvidersRoute);
            return Task.CompletedTask;
        }

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            CancelQrExpiry();
            ViewState.RedirectUrl = null;
            ViewState.IsQrExpired = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            _appLifecycleService.Resumed -= OnAppResumed;
            _appLifecycleService.Resumed += OnAppResumed;
            await _appLifecycleService.StartWatchingAsync();

            try
            {
                await LoadQrAsync(cancellationToken);
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            _appLifecycleService.Resumed -= OnAppResumed;
            CancelQrExpiry();
            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        private async Task LoadQrAsync(CancellationToken cancellationToken)
        {
            var provider = _registrationSession.SelectedProvider;
            var methodId = provider?.MethodId ?? 0;
            var channelGuid = provider?.ChannelGuid ?? Guid.Empty;

            try
            {
                var result = await _registrationService.StartAsync(new RegistrationStartRequest
                {
                    MethodId = methodId,
                    Kind = RegistrationStartKind.Redirect
                }, cancellationToken);

                switch (result)
                {
                    case RegistrationStartResult.RedirectRequired r:
                        _registrationSession.SetStartResult(
                            r.Token,
                            string.Empty,
                            0,
                            r.ExpiresInSeconds,
                            RegistrationFlow.Redirect);
                        ViewState.RedirectUrl = r.RedirectUrl;
                        await OpenRedirectAsync(r.RedirectUrl, cancellationToken);
                        StartQrExpiryTimer();
                        _ = StartTokenPollingAsync(_qrExpiryCts!.Token);
                        break;

                    default:
                        Logger.LogWarning("Redirect registration start failed: {Result}", result);
                        _registrationSession.SetFailedProviderChannelGuid(channelGuid);
                        NavigationService.NavigateTo(NavigationHelper.RegistrationProvidersRoute);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Redirect registration start error.");
                _registrationSession.SetFailedProviderChannelGuid(channelGuid);
                NavigationService.NavigateTo(NavigationHelper.RegistrationProvidersRoute);
            }
        }

        // mobile deviation: the QR code is replaced by opening the link (or the manual "Open" button)
        public async Task OpenAsync()
        {
            if (string.IsNullOrEmpty(ViewState.RedirectUrl))
                return;

            try
            {
                await _externalLauncher.OpenAsync(ViewState.RedirectUrl);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cannot open the registration redirect link.");
            }
        }

        private async Task OpenRedirectAsync(string redirectUrl, CancellationToken cancellationToken)
        {
            try
            {
                await _externalLauncher.RedirectPlaceholderAsync(null, redirectUrl, cancellationToken);
            }
            catch (Exception ex)
            {
                // a blocked launch is not fatal, the page offers the manual button
                Logger.LogWarning(ex, "Cannot open the registration redirect link automatically.");
            }
        }

        private async void OnAppResumed(object? sender, EventArgs e)
        {
            var cancellationToken = _qrExpiryCts?.Token;
            if (cancellationToken is null)
                return;

            try
            {
                await CheckTokenConfirmedAsync(cancellationToken.Value);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Token confirmation check on app resume failed.");
            }
        }

        private async Task StartTokenPollingAsync(CancellationToken cancellationToken)
        {
            var transientFailureCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (await CheckTokenConfirmedAsync(cancellationToken))
                        return;

                    transientFailureCount = 0;

                    await Task.Delay(TokenPollInterval, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    if (transientFailureCount < MaxTransientFailureRetries)
                    {
                        transientFailureCount++;
                        Logger.LogWarning(ex, "Registration token status check failed. Retry {Attempt}/{MaxAttempts}.", transientFailureCount, MaxTransientFailureRetries);

                        try
                        {
                            await Task.Delay(TransientFailureRetryDelay, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }

                        continue;
                    }

                    Logger.LogError(ex, "Registration token status check failed.");
                    CancelQrExpiry();
                    NavigationService.NavigateTo(NavigationHelper.RegistrationErrorRoute);
                    return;
                }
            }
        }

        /// <returns>true when the token is resolved and the navigation was performed.</returns>
        private async Task<bool> CheckTokenConfirmedAsync(CancellationToken cancellationToken)
        {
            if (_isChecking)
                return false;

            _isChecking = true;
            try
            {
                var result = await _registrationService.IsTokenConfirmedAsync(
                    _registrationSession.Token, cancellationToken);

                if (result.UserAlreadyExists)
                {
                    CancelQrExpiry();
                    NavigationService.NavigateTo(NavigationHelper.LoginPage);
                    return true;
                }

                if (result.IsConfirmed)
                {
                    CancelQrExpiry();

                    if (!string.IsNullOrEmpty(result.Phone))
                    {
                        var confirmedPhone = result.Phone.StartsWith("+")
                            ? result.Phone.Substring(1)
                            : result.Phone;

                        _registrationSession.SetContactDetails(confirmedPhone);
                    }

                    NavigationService.NavigateTo(NavigationHelper.RegistrationRedirectSuccessRoute);
                    return true;
                }

                return false;
            }
            finally
            {
                _isChecking = false;
            }
        }

        private void StartQrExpiryTimer()
        {
            CancelQrExpiry();
            _qrExpiryCts = new CancellationTokenSource();
            var token = _qrExpiryCts.Token;
            _ = ExpireQrAfterDelayAsync(token);
        }

        private async Task ExpireQrAfterDelayAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(QrExpiryDelay, cancellationToken);
                ViewState.IsQrExpired = true;
                ViewState.RaiseChanged();
                CancelQrExpiry();
            }
            catch (OperationCanceledException)
            {
                // Navigation or refresh cancelled the timer.
            }
        }

        private void CancelQrExpiry()
        {
            if (_qrExpiryCts is null)
                return;

            _qrExpiryCts.Cancel();
            _qrExpiryCts.Dispose();
            _qrExpiryCts = null;
        }
    }
}
