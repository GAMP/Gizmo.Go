using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationCallSuccess)]
    public sealed class RegistrationCallSuccessViewService : ViewStateServiceBase<RegistrationCallSuccessViewState>
    {
        public RegistrationCallSuccessViewService(
            RegistrationCallSuccessViewState viewState,
            ILogger<RegistrationCallSuccessViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
    }
}
