using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    /// <summary>
    /// Registration via phone call has no flow in the client (CallRequired falls back to the providers step).
    /// </summary>
    [Register()]
    [Route(NavigationHelper.RegistrationCallVerifyRoute)]
    public sealed class UserRegistrationCallVerifyViewService : ViewStateServiceBase<UserRegistrationCallVerifyViewState>
    {
        public UserRegistrationCallVerifyViewService(
            UserRegistrationCallVerifyViewState viewState,
            ILogger<UserRegistrationCallVerifyViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }

        public ValueTask CancelAsync()
        {
            NavigationService.NavigateTo(NavigationHelper.RegistrationProvidersRoute);
            return ValueTask.CompletedTask;
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            NavigationService.NavigateTo(NavigationHelper.RegistrationProvidersRoute);
            return Task.CompletedTask;
        }
    }
}
