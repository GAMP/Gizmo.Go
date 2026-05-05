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

        private readonly CountdownTimer _timer = new();

        public ValueTask CancelAsync()
        {
            CancelTimer();
            _navigationService.NavigateTo(NavigationHelper.RegistrationCallPhone);
            return ValueTask.CompletedTask;
        }

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

        private Task StartTimerAsync()
            => _timer.StartAsync(120, secs =>
            {
                ViewState.SecondsLeft = secs;
                ViewState.RaiseChanged();
                return Task.CompletedTask;
            }, Logger);

        private void CancelTimer() => _timer.Cancel();
    }
}
