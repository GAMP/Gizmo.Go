using Gizmo.Go.UI.View.States;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components
{
    /// <summary>
    /// Logo above the auth card title (login, registration providers).
    /// Shows the configured logo image or the empty placeholder square when none is set.
    /// </summary>
    public partial class AuthCardLogo : ComponentBase, IDisposable
    {
        [Inject] private RegistrationConfigurationViewState RegistrationConfigurationViewState { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationConfigurationViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationConfigurationViewState);
        }
    }
}
