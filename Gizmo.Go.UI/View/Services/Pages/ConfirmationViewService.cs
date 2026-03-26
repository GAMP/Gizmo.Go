using System.Web;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages
{
    [Register()]
    [Route("/confirmation")]
    public sealed class ConfirmationViewService : ViewStateServiceBase<ConfirmationViewState>
    {
        #region FIELDS

        private CancellationTokenSource? _timerCts;

        #endregion

        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;

        public ConfirmationViewService(
            ConfirmationViewState viewState,
            ILogger<ConfirmationViewService> logger,
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

        public ValueTask ConfirmAsync()
        {
            if (!ViewState.CanSubmit)
                return ValueTask.CompletedTask;

            CancelTimer();
            _navigationService.NavigateTo("/create-password");
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateBackAsync()
        {
            CancelTimer();
            _navigationService.NavigateTo("/create-account");
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
            var currentUri = _navigationService.GetUri();
            if (!string.IsNullOrEmpty(currentUri))
            {
                var uri = new Uri(currentUri);
                var phone = HttpUtility.ParseQueryString(uri.Query).Get("phone");
                ViewState.Phone = !string.IsNullOrEmpty(phone) ? Uri.UnescapeDataString(phone) : "+7**********";
            }

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
