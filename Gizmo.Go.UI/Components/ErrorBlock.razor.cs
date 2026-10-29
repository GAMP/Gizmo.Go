using Gizmo.Go.UI.Services.Common;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components
{
    public partial class ErrorBlock : ComponentBase
    {
        [Inject]
        private IAuthFlowService AuthFlowService { get; set; } = null!;

        [Parameter]
        public string? Message { get; set; }

        /// <summary>
        /// Shows a "Go to home" link that leaves the registration / recovery flow (sessions are cleared).
        /// Used on screens with a back button, so the user does not have to go back step by step.
        /// </summary>
        [Parameter]
        public bool ShowHomeLink { get; set; }

        /// <summary>
        /// Shows a "Try again" link that runs the callback (e.g. reloads the server options of the screen).
        /// </summary>
        [Parameter]
        public EventCallback OnRetry { get; set; }

        private void GoHome() => AuthFlowService.Leave();
    }
}
