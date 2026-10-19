using Gizmo.Go.Core.Models.PasswordRecovery;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.PasswordRecovery;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States.Pages.PasswordRecovery;
using Gizmo.Go.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.UI;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.PasswordRecovery
{
    [Register]
    [Route(NavigationHelper.PasswordRecoveryConfirmationRoute)]
    public sealed class PasswordRecoveryConfirmationViewService : ValidatingViewStateServiceBase<PasswordRecoveryConfirmationViewState>
    {
        private static readonly TimeSpan QrExpiryDelay = TimeSpan.FromMinutes(4);
        private static readonly TimeSpan TokenPollInterval = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan TransientFailureRetryDelay = TimeSpan.FromSeconds(1);
        private const int MaxTransientFailureRetries = 1;

        private readonly IPasswordRecoveryService _passwordRecoveryService;
        private readonly IPasswordRecoverySessionService _session;
        private readonly ILocalizationService _localizationService;
        private readonly IExternalLauncher _externalLauncher;
        private readonly IAppLifecycleService _appLifecycleService;
        private readonly CountdownTimer _timer = new();
        private CancellationTokenSource? _asyncActionCts;
        private bool _isChecking;

        public PasswordRecoveryConfirmationViewService(
            PasswordRecoveryConfirmationViewState viewState,
            ILogger<PasswordRecoveryConfirmationViewService> logger,
            IServiceProvider serviceProvider,
            IPasswordRecoveryService passwordRecoveryService,
            IPasswordRecoverySessionService session,
            ILocalizationService localizationService,
            IExternalLauncher externalLauncher,
            IAppLifecycleService appLifecycleService) : base(viewState, logger, serviceProvider)
        {
            _passwordRecoveryService = passwordRecoveryService;
            _session = session;
            _localizationService = localizationService;
            _externalLauncher = externalLauncher;
            _appLifecycleService = appLifecycleService;
        }

        public void SetConfirmationCode(string value)
        {
            ViewState.ConfirmationCode = value;
            ValidateProperty(() => ViewState.ConfirmationCode);
        }

        public ValueTask SetDigitAsync(int index, string raw)
        {
            if (index < 0 || index >= ViewState.Digits.Length)
                return ValueTask.CompletedTask;

            ViewState.Digits[index] = new string(raw.Where(char.IsLetterOrDigit).Take(1).ToArray());
            SetConfirmationCode(string.Concat(ViewState.Digits));
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask ClearDigitAsync(int index)
        {
            if (index < 0 || index >= ViewState.Digits.Length)
                return ValueTask.CompletedTask;

            ViewState.Digits[index] = string.Empty;
            SetConfirmationCode(string.Concat(ViewState.Digits));
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public Task NavigateBackAsync()
        {
            NavigationService.NavigateTo(NavigationHelper.PasswordRecoveryRoute);
            return Task.CompletedTask;
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
                Logger.LogError(ex, "Cannot open the password recovery redirect link.");
            }
        }

        public async Task Confirm()
        {
            if (ViewState.IsLoading)
                return;

            ViewState.IsLoading = true;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            Validate();

            if (ViewState.IsValid != true)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
                return;
            }

            try
            {
                var result = await _passwordRecoveryService.ConfirmCodeAsync(_session.Token, ViewState.ConfirmationCode);

                switch (result)
                {
                    case PasswordRecoveryConfirmCode.Success:
                        _session.SetTokenConfirmed(true);
                        NavigationService.NavigateTo(NavigationHelper.PasswordRecoverySetNewPasswordRoute);
                        break;

                    case PasswordRecoveryConfirmCode.InvalidToken:
                    case PasswordRecoveryConfirmCode.ExpiredToken:
                    case PasswordRecoveryConfirmCode.UsedToken:
                    case PasswordRecoveryConfirmCode.InvalidConfirmationCode:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_USER_CONFIRMATION_CONFIRMATION_CODE_IS_INVALID));
                        break;

                    default:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_AN_ERROR_HAS_OCCURRED));
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Confirm password recovery code error.");
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_AN_ERROR_HAS_OCCURRED));
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public async ValueTask RestartTimerAsync()
        {
            if (ViewState.IsLoading)
                return;

            if (_session.Action != PasswordRecoveryAction.Code)
                return;

            ViewState.IsLoading = true;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            try
            {
                var provider = _session.ActiveProvider;
                var identifierKind = _session.IdentifierKind;
                if (provider is null || identifierKind is null)
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_AN_ERROR_HAS_OCCURRED));
                    return;
                }

                var result = await _passwordRecoveryService.StartAsync(new PasswordRecoveryStartRequest
                {
                    MethodId = provider.MethodId,
                    IdentifierKind = identifierKind.Value,
                    Value = _session.MatchValue
                });

                if (result is PasswordRecoveryStartResult.CodeInputRequired r)
                {
                    _session.SetStartResult(
                        r.Token,
                        r.Destination ?? _session.Destination,
                        r.CodeLength,
                        r.ExpiresInSeconds);
                    _ = StartTimerAsync();
                }
                else
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_AN_ERROR_HAS_OCCURRED));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Resend recovery code error.");
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_AN_ERROR_HAS_OCCURRED));
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
        }

        public void Clear()
        {
            ViewState.ConfirmationCode = string.Empty;
            ViewState.ConfirmationCodeMessage = string.Empty;
            ViewState.Digits = [];
            ViewState.Action = PasswordRecoveryAction.None;
            ViewState.RedirectUrl = null;
            ViewState.CallPhoneNumber = null;
            ViewState.IsQrExpired = false;
            ViewState.SecondsLeft = 0;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ResetValidationState();
            DebounceViewStateChanged();
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            CancelTimer();
            CancelAsyncActionPolling();

            if (string.IsNullOrEmpty(_session.Token))
            {
                NavigationService.NavigateTo(NavigationHelper.PasswordRecoveryRoute);
                return;
            }

            ViewState.ConfirmationCode = string.Empty;
            ViewState.Digits = CreateDigits(_session.CodeLength);
            ViewState.Action = _session.Action;
            ViewState.RedirectUrl = _session.RedirectUrl;
            ViewState.CallPhoneNumber = _session.CallPhoneNumber;
            ViewState.IsQrExpired = false;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ResetValidationState();

            switch (_session.Action)
            {
                case PasswordRecoveryAction.Code:
                    var channel = _session.ActiveProvider?.Channel ?? PasswordRecoveryChannel.Email;
                    ViewState.ConfirmationCodeMessage = channel == PasswordRecoveryChannel.Email
                        ? _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_USER_CONFIRMATION_EMAIL_MESSAGE), _session.Destination)
                        : _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_PASSWORD_RECOVERY_PLEASE_ENTER_RECOVERY_CODE), _session.Destination);
                    ViewState.RaiseChanged();
                    _ = StartTimerAsync();
                    break;

                case PasswordRecoveryAction.Redirect:
                    if (string.IsNullOrEmpty(_session.RedirectUrl))
                    {
                        Logger.LogWarning("Password recovery redirect result carried no redirect url.");
                        NavigateBackWithFailure();
                        break;
                    }

                    ViewState.ConfirmationCodeMessage = string.Empty;
                    ViewState.RaiseChanged();
                    await OpenRedirectAsync(_session.RedirectUrl, cancellationToken);
                    StartAsyncActionPolling();
                    StartQrExpiryTimer();
                    break;

                case PasswordRecoveryAction.Call:
                    ViewState.ConfirmationCodeMessage = string.Empty;
                    ViewState.RaiseChanged();
                    StartAsyncActionPolling();
                    break;

                default:
                    NavigationService.NavigateTo(NavigationHelper.PasswordRecoveryRoute);
                    break;
            }

            if (_session.Action is PasswordRecoveryAction.Redirect or PasswordRecoveryAction.Call)
            {
                _appLifecycleService.Resumed -= OnAppResumed;
                _appLifecycleService.Resumed += OnAppResumed;
                await _appLifecycleService.StartWatchingAsync();
            }
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            _appLifecycleService.Resumed -= OnAppResumed;
            CancelTimer();
            CancelAsyncActionPolling();
            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.ConfirmationCode) &&
                ViewState.ConfirmationCode.Length != _session.CodeLength)
            {
                AddError(() => ViewState.ConfirmationCode, _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_CONFIRMATION_CODE_LENGTH_ERROR), _session.CodeLength));
            }
        }

        private Task StartTimerAsync()
        {
            const int seconds = 90;
            return _timer.StartAsync(seconds, secs =>
            {
                ViewState.SecondsLeft = secs;
                ViewState.RaiseChanged();
                return Task.CompletedTask;
            }, Logger);
        }

        private void CancelTimer() => _timer.Cancel();

        private void StartAsyncActionPolling()
        {
            CancelAsyncActionPolling();
            _asyncActionCts = new CancellationTokenSource();
            _ = PollTokenConfirmationAsync(_asyncActionCts.Token);
        }

        private static string[] CreateDigits(int codeLength)
        {
            var digits = new string[codeLength > 0 ? codeLength : 6];
            Array.Fill(digits, string.Empty);
            return digits;
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
                Logger.LogWarning(ex, "Cannot open the password recovery redirect link automatically.");
            }
        }

        private async void OnAppResumed(object? sender, EventArgs e)
        {
            var cancellationToken = _asyncActionCts?.Token;
            if (cancellationToken is null)
                return;

            try
            {
                await CheckTokenConfirmedAsync(cancellationToken.Value);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Password recovery token check on app resume failed.");
            }
        }

        private async Task PollTokenConfirmationAsync(CancellationToken cancellationToken)
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
                        Logger.LogWarning(ex, "Password recovery token polling failed. Retrying {Attempt}/{MaxAttempts}.", transientFailureCount, MaxTransientFailureRetries);

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

                    Logger.LogError(ex, "Password recovery token polling failed.");
                    NavigateBackWithFailure();
                    return;
                }
            }
        }

        /// <returns>true when the token is confirmed and the navigation was performed.</returns>
        private async Task<bool> CheckTokenConfirmedAsync(CancellationToken cancellationToken)
        {
            if (_isChecking)
                return false;

            _isChecking = true;
            try
            {
                var result = await _passwordRecoveryService.IsTokenConfirmedAsync(_session.Token, cancellationToken);

                if (!result.IsConfirmed)
                    return false;

                _session.SetTokenConfirmed(true);
                CancelAsyncActionPolling();
                NavigationService.NavigateTo(NavigationHelper.PasswordRecoverySetNewPasswordRoute);
                return true;
            }
            finally
            {
                _isChecking = false;
            }
        }

        private void StartQrExpiryTimer()
        {
            var token = _asyncActionCts?.Token;
            if (token is null)
                return;

            _ = ExpireQrAfterDelayAsync(token.Value);
        }

        private async Task ExpireQrAfterDelayAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(QrExpiryDelay, cancellationToken);
                ViewState.IsQrExpired = true;
                ViewState.RaiseChanged();
                CancelAsyncActionPolling();
            }
            catch (OperationCanceledException)
            {
                // Navigation or successful confirmation cancelled the timer.
            }
        }

        private void NavigateBackWithFailure()
        {
            var channelGuid = _session.ActiveProvider?.ChannelGuid;
            _session.SetFailedProviderChannelGuid(channelGuid);
            _session.SetShowAllProviders(true);
            CancelAsyncActionPolling();
            NavigationService.NavigateTo(NavigationHelper.PasswordRecoveryRoute);
        }

        private void CancelAsyncActionPolling()
        {
            if (_asyncActionCts is null)
                return;

            _asyncActionCts.Cancel();
            _asyncActionCts.Dispose();
            _asyncActionCts = null;
        }
    }
}
