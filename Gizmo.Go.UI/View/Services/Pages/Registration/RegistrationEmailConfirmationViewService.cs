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
    [Route(NavigationHelper.RegistrationEmailConfirmation)]
    public sealed class RegistrationEmailConfirmationViewService : ViewStateServiceBase<RegistrationEmailConfirmationViewState>
    {
        private readonly CountdownTimer _timer = new();

        private readonly NavigationService _navigationService;
        private readonly IConfirmationService _confirmationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly ILocalizationService _localizationService;

        public RegistrationEmailConfirmationViewService(
            RegistrationEmailConfirmationViewState viewState,
            ILogger<RegistrationEmailConfirmationViewService> logger,
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
                Logger.LogError(ex, "Email confirmation request failed.");
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
                _navigationService.NavigateTo(NavigationHelper.RegistrationEmailAddPhone);
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
            _navigationService.NavigateTo(NavigationHelper.RegistrationEmail);
            return ValueTask.CompletedTask;
        }

        // Restarts the countdown only — does NOT resend the email code. Real resend requires a new StartAsync call (Task 2.8 variant B).
        public ValueTask RestartTimerAsync()
        {
            _ = StartTimerAsync();
            return ValueTask.CompletedTask;
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (!_registrationSession.HasToken)
            {
                _navigationService.NavigateTo(NavigationHelper.RegistrationEmail);
                return base.OnNavigatedIn(navigationParameters, cancellationToken);
            }

            if (_registrationSession.CodeLength == 0)
            {
                _registrationSession.Clear();
                _navigationService.NavigateTo(NavigationHelper.RegistrationProviders + "?error=1");
                return base.OnNavigatedIn(navigationParameters, cancellationToken);
            }

            var codeLength = Math.Clamp(_registrationSession.CodeLength, 4, 6);
            var digits = new string[codeLength];
            Array.Fill(digits, string.Empty);

            ViewState.Token = _registrationSession.Token;
            ViewState.Email = _registrationSession.Email;
            ViewState.ErrorMessage = null;
            ViewState.IsSubmitting = false;
            ViewState.CodeLength = codeLength;
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

        private Task StartTimerAsync()
            => _timer.StartAsync(60, secs =>
            {
                ViewState.SecondsLeft = secs;
                ViewState.RaiseChanged();
                return Task.CompletedTask;
            }, Logger);

        private void CancelTimer() => _timer.Cancel();
    }
}
