using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.PasswordRecovery;
using Gizmo.Go.UI.View.States.Pages.PasswordRecovery;
using Gizmo.UI.View.Services;
using Gizmo.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.PasswordRecovery
{
    /// <summary>
    /// Account search result between the identifier entry and the provider choice.
    /// Mobile deviation: Figma 159:14156 (found), 159:14187 (not found).
    /// </summary>
    [Register()]
    [Route(NavigationHelper.PasswordRecoveryAccountRoute)]
    public sealed class PasswordRecoveryAccountViewService : ViewStateServiceBase<PasswordRecoveryAccountViewState>
    {
        private readonly IPasswordRecoverySessionService _session;

        public PasswordRecoveryAccountViewService(
            PasswordRecoveryAccountViewState viewState,
            ILogger<PasswordRecoveryAccountViewService> logger,
            IServiceProvider serviceProvider,
            IPasswordRecoverySessionService session) : base(viewState, logger, serviceProvider)
        {
            _session = session;
        }

        // the provider step is shown for any found account, even with a single method
        public Task ContinueAsync()
        {
            NavigationService.NavigateTo(NavigationHelper.PasswordRecoveryRoute);
            return Task.CompletedTask;
        }

        // not found: start over from the identifier kind choice with a clean session
        public Task ChooseAnotherMethodAsync()
        {
            _session.Clear();
            NavigationService.NavigateTo(NavigationHelper.PasswordRecoveryKindRoute);
            return Task.CompletedTask;
        }

        // back to the identifier entry to correct the value; the chosen identifier kind is kept
        public Task NavigateBackAsync()
        {
            _session.ResetSearch();
            NavigationService.NavigateTo(NavigationHelper.PasswordRecoveryDestinationRoute);
            return Task.CompletedTask;
        }

        public Task NavigateToLoginAsync()
        {
            NavigationService.NavigateTo(NavigationHelper.LoginPage);
            return Task.CompletedTask;
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (_session.IdentifierKind is null || _session.IsAccountFound is null)
            {
                NavigationService.NavigateTo(NavigationHelper.PasswordRecoveryKindRoute);
                return Task.CompletedTask;
            }

            ViewState.IsFound = _session.IsAccountFound.Value;
            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }
    }
}
