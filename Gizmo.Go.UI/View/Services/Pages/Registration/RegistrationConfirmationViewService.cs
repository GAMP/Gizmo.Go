using Gizmo.Go.Core.Models.Confirmation;
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
    [Route(NavigationHelper.RegistrationConfirmationPage)]
    public sealed class RegistrationConfirmationViewService : ViewStateServiceBase<RegistrationConfirmationViewState>
    {
        #region FIELDS

        private CancellationTokenSource? _timerCts;

        #endregion

        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;
        private readonly IConfirmationService _confirmationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly ILocalizationService _localizationService;

        public RegistrationConfirmationViewService(
            RegistrationConfirmationViewState viewState,
            ILogger<RegistrationConfirmationViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IConfirmationService confirmationService,
            IRegistrationSessionService registrationSession,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _confirmationService = confirmationService;
            _registrationSession = registrationSession;
            _localizationService = localizationService;
        }

        #endregion

        #region METHODS

        public ValueTask SetDigitAsync(int index, string raw)
        {
            if (index < 0 || index >= ViewState.Digits.Length)
                return ValueTask.CompletedTask;

            ViewState.Digits[index] = new string(raw.Where(char.IsDigit).Take(1).ToArray());
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask ClearDigitAsync(int index)
        {
            if (index < 0 || index >= ViewState.Digits.Length)
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
                ViewState.RaiseChanged();
                _registrationSession.Clear();
                CancelTimer();
                _navigationService.NavigateTo(NavigationHelper.RegistrationProviders + "?error=1");
                return;
            }

            ViewState.IsSubmitting = false;

            if (result.Result == TokenConfirmationResultCode.Success)
            {
                CancelTimer();
                _navigationService.NavigateTo(NavigationHelper.RegistrationPasswordPage);
                return;
            }

            if (result.Result == TokenConfirmationResultCode.InvalidConfirmationCode)
            {
                ViewState.ErrorMessage = _localizationService.GetString(ConfirmationErrorHelper.GetLocalizationKey(result.Result));
                ViewState.RaiseChanged();
                return;
            }

            _registrationSession.Clear();
            CancelTimer();
            _navigationService.NavigateTo(NavigationHelper.RegistrationProviders + "?error=1");
        }

        public ValueTask NavigateBackAsync()
        {
            CancelTimer();
            _navigationService.NavigateTo(NavigationHelper.RegistrationPhone);
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
                _navigationService.NavigateTo(NavigationHelper.RegistrationPhone);
                return base.OnNavigatedIn(navigationParameters, cancellationToken);
            }

            ViewState.Token = _registrationSession.Token;
            ViewState.MaskedPhone = MaskPhone(_registrationSession.Phone);
            ViewState.ErrorMessage = null;
            ViewState.IsSubmitting = false;
            var digits = new string[_registrationSession.CodeLength];
            Array.Fill(digits, string.Empty);
            ViewState.CodeLength = _registrationSession.CodeLength;
            ViewState.Digits = digits;
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

        private static string MaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return string.Empty;

            var digits = new string(phone.Where(char.IsDigit).ToArray());
            var visibleCount = Math.Min(5, digits.Length);
            var prefix = phone.StartsWith('+') ? "+" : string.Empty;

            return prefix + digits[..visibleCount] + new string('*', digits.Length - visibleCount);
        }

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
