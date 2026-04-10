using Gizmo.Go.Core.Models.Confirmation;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages
{
    [Register()]
    [Route(NavigationHelper.ConfirmationPage)]
    public sealed class ConfirmationViewService : ViewStateServiceBase<ConfirmationViewState>
    {
        #region FIELDS

        private CancellationTokenSource? _timerCts;

        #endregion

        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;
        private readonly IConfirmationService _confirmationService;
        private readonly IRegistrationSessionService _registrationSession;

        public ConfirmationViewService(
            ConfirmationViewState viewState,
            ILogger<ConfirmationViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IConfirmationService confirmationService,
            IRegistrationSessionService registrationSession) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _confirmationService = confirmationService;
            _registrationSession = registrationSession;
        }

        #endregion

        #region METHODS

        public ValueTask SetDigitAsync(int index, string raw)
        {
            if (index < 0 || index >= 6)
                return ValueTask.CompletedTask;

            ViewState.Digits[index] = new string(raw.Where(char.IsDigit).Take(1).ToArray());
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask ClearDigitAsync(int index)
        {
            if (index < 0 || index >= 6)
                return ValueTask.CompletedTask;

            ViewState.Digits[index] = string.Empty;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public async ValueTask ConfirmAsync(CancellationToken cancellationToken = default)
        {
            if (!ViewState.CanSubmit || ViewState.IsSubmitting)
                return;

            ViewState.IsSubmitting = true;
            ViewState.ErrorCode = null;
            ViewState.RaiseChanged();

            var request = new TokenConfirmationRequest
            {
                Token = ViewState.Token,
                ConfirmationCode = string.Concat(ViewState.Digits)
            };

            TokenConfirmationResult result;
            try
            {
                result = await _confirmationService.ConfirmAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Confirmation request failed.");
                ViewState.IsSubmitting = false;
                ViewState.ErrorCode = TokenConfirmationResultCode.Unknown;
                ViewState.RaiseChanged();
                return;
            }

            ViewState.IsSubmitting = false;

            if (result.Result == TokenConfirmationResultCode.Success)
            {
                CancelTimer();
                _navigationService.NavigateTo(NavigationHelper.CreatePasswordPage);
                return;
            }

            ViewState.ErrorCode = result.Result;
            ViewState.RaiseChanged();
        }

        public ValueTask NavigateBackAsync()
        {
            CancelTimer();
            _navigationService.NavigateTo(NavigationHelper.CreateAccount);
            return ValueTask.CompletedTask;
        }

        public ValueTask RestartTimerAsync()
        {
            _ = StartTimerAsync();
            return ValueTask.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (!_registrationSession.HasToken)
            {
                _navigationService.NavigateTo(NavigationHelper.CreateAccount);
                return base.OnNavigatedIn(navigationParameters, cancellationToken);
            }

            ViewState.Token = _registrationSession.Token;
            ViewState.ErrorCode = null;
            ViewState.IsSubmitting = false;
            ViewState.Digits = new string[] { "", "", "", "", "", "" };
            ViewState.RaiseChanged();

            _ = StartTimerAsync();

            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            CancelTimer();
            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        #endregion

        #region PRIVATE METHODS

        private async Task StartTimerAsync()
        {
            CancelTimer();
            _timerCts = new CancellationTokenSource();
            var token = _timerCts.Token;

            ViewState.SecondsLeft = 60;
            ViewState.RaiseChanged();

            try
            {
                while (ViewState.SecondsLeft > 0 && !token.IsCancellationRequested)
                {
                    await Task.Delay(1000, token);

                    if (token.IsCancellationRequested)
                        break;

                    ViewState.SecondsLeft--;
                    ViewState.RaiseChanged();
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Confirmation timer faulted.");
            }
        }

        private void CancelTimer()
        {
            _timerCts?.Cancel();
            _timerCts?.Dispose();
            _timerCts = null;
        }

        #endregion
    }
}
