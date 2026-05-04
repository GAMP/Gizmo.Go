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
    [Route(NavigationHelper.RegistrationCallVerify)]
    public sealed class RegistrationCallVerifyViewService : ViewStateServiceBase<RegistrationCallVerifyViewState>
    {
        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly IPhoneValidationService _phoneValidationService;

        public RegistrationCallVerifyViewService(
            RegistrationCallVerifyViewState viewState,
            ILogger<RegistrationCallVerifyViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IRegistrationSessionService registrationSession,
            IPhoneValidationService phoneValidationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _registrationSession = registrationSession;
            _phoneValidationService = phoneValidationService;
        }

        #endregion

        #region FIELDS

        private CancellationTokenSource? _timerCts;

        #endregion

        #region METHODS

        public ValueTask CancelAsync()
        {
            CancelTimer();
            _navigationService.NavigateTo(NavigationHelper.RegistrationCallPhone);
            return ValueTask.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            // TODO: получить ServerPhoneNumber из ответа API call-верификации
            ViewState.ServerPhoneNumber = string.Empty;
            ViewState.MaskedPhone = _phoneValidationService.MaskPhone(_registrationSession.Phone);
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

            ViewState.SecondsLeft = 120;
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
                Logger.LogError(ex, "Call verify timer faulted.");
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
