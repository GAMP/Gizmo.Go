using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationCallSuccess : ComponentBase, IDisposable
    {
        [Inject] private UserRegistrationCallSuccessViewState UserRegistrationCallSuccessViewState { get; set; } = null!;

        [Inject] private UserRegistrationCallSuccessViewService UserRegistrationCallSuccessViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationCallSuccessViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationCallSuccessViewState);
        }
    }
}
