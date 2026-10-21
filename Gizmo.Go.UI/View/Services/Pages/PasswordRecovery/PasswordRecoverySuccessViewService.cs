using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.View.States.Pages.PasswordRecovery;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.PasswordRecovery
{
    [Register()]
    [Route(NavigationHelper.PasswordRecoverySuccessRoute)]
    public sealed class PasswordRecoverySuccessViewService : ViewStateServiceBase<PasswordRecoverySuccessViewState>
    {
        public PasswordRecoverySuccessViewService(
            PasswordRecoverySuccessViewState viewState,
            ILogger<PasswordRecoverySuccessViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }

        public Task NavigateToLoginAsync()
        {
            NavigationService.NavigateTo(NavigationHelper.LoginPage);
            return Task.CompletedTask;
        }
    }
}
