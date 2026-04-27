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

            await base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters,
            CancellationToken cancellationToken = default)
        {
            _appLifecycleService.Resumed -= OnAppResumed;
            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        #endregion

        #region METHODS

        public ValueTask NavigateBackAsync()
        {
            _registrationSession.Clear();
            _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
            return ValueTask.CompletedTask;
        }

        #endregion

        #region PRIVATE METHODS

        private async void OnAppResumed(object? sender, EventArgs e)
        {
            if (_isChecking)
                return;

            _isChecking = true;
            try
            {
                var result = await _registrationService.IsTokenConfirmedAsync(_registrationSession.Token);
                if (result.IsConfirmed)
                {
                    if (!string.IsNullOrEmpty(result.Phone))
                        _registrationSession.SetPhone(result.Phone);
                    _navigationService.NavigateTo(NavigationHelper.RegistrationBotVerifySuccess);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Token confirmation check failed.");
                _registrationSession.Clear();
                _navigationService.NavigateTo(NavigationHelper.RegistrationProviders + "?error=1");
            }
            finally
            {
                _isChecking = false;
            }
        }

        #endregion
    }
}
