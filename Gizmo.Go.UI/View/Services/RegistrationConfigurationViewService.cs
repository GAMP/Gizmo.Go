using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services
{
    /// <summary>
    /// Registration configuration view state service.
    /// </summary>
    [Register()]
    public sealed class RegistrationConfigurationViewService : ViewStateServiceBase<RegistrationConfigurationViewState>
    {
        private readonly IRegistrationOptionsService _registrationOptionsService;
        private readonly IUserRegistrationService _registrationService;

        public RegistrationConfigurationViewService(
            RegistrationConfigurationViewState viewState,
            IRegistrationOptionsService registrationOptionsService,
            IUserRegistrationService registrationService,
            ILogger<RegistrationConfigurationViewService> logger,
            IServiceProvider serviceProvider)
            : base(viewState, logger, serviceProvider)
        {
            _registrationOptionsService = registrationOptionsService;
            _registrationService = registrationService;
        }

        protected override async Task OnInitializing(CancellationToken ct)
        {
            try
            {
                var options = await _registrationOptionsService.GetAsync(ct).ConfigureAwait(false);

                ViewState.IsDirectEnabled = options.IsDirectRegistrationEnabled;
                ViewState.IsPasswordRecoveryEnabled = options.IsPasswordRecoveryEnabled;
                ViewState.Logo = options.Logo;

                //If there is no default user group this will fail.
                await _registrationService.GetRequiredUserInfoAsync(ct).ConfigureAwait(false);

                ViewState.IsEnabled = options.IsRegistrationEnabled;
            }
            catch (Exception ex)
            {
                ViewState.IsEnabled = false;
                Logger.LogError(ex, "Could not determine if registration is enabled.");
            }

            ViewState.RaiseChanged();
        }
    }
}
