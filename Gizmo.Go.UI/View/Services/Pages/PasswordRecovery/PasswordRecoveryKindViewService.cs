using Gizmo.Go.Core.Models.PasswordRecovery;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.PasswordRecovery;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States.Pages.PasswordRecovery;
using Gizmo.Go.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.PasswordRecovery
{
    [Register]
    [Route(NavigationHelper.PasswordRecoveryKindRoute)]
    public sealed class PasswordRecoveryKindViewService : ViewStateServiceBase<PasswordRecoveryKindViewState>
    {
        private readonly IPasswordRecoverySessionService _session;

        public PasswordRecoveryKindViewService(
            PasswordRecoveryKindViewState viewState,
            ILogger<PasswordRecoveryKindViewService> logger,
            IServiceProvider serviceProvider,
            IPasswordRecoverySessionService session) : base(viewState, logger, serviceProvider)
        {
            _session = session;
        }

        public void SelectKind(PasswordRecoveryIdentifierKind kind)
        {
            _session.SetIdentifierKind(kind);
            NavigationService.NavigateTo(NavigationHelper.PasswordRecoveryDestinationRoute);
        }

        public Task NavigateBackAsync()
        {
            NavigationService.NavigateTo(NavigationHelper.LoginPage);
            return Task.CompletedTask;
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            _session.Clear();
            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }
    }
}
