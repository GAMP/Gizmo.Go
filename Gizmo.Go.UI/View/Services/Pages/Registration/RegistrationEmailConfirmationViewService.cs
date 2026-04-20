using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationEmailConfirmation)]
    public sealed class RegistrationEmailConfirmationViewService : ViewStateServiceBase<RegistrationEmailConfirmationViewState>
    {
        #region FIELDS

        private CancellationTokenSource? _timerCts;

        #endregion

        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;

        public RegistrationEmailConfirmationViewService(
            RegistrationEmailConfirmationViewState viewState,
            ILogger<RegistrationEmailConfirmationViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
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
            ViewState.ErrorMessage = null;
            ViewState.RaiseChanged();

            // TODO: wire up to email confirmation API
            await Task.Delay(0, cancellationToken);

            ViewState.IsSubmitting = false;
            ViewState.RaiseChanged();
        }

        public ValueTask NavigateBackAsync()
        {
            CancelTimer();
            _navigationService.NavigateTo(NavigationHelper.RegistrationEmail);
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
            ViewState.ErrorMessage = null;
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
                Logger.LogError(ex, "Email confirmation timer faulted.");
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
