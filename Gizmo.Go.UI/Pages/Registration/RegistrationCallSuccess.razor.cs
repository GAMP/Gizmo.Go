using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationCallSuccess : ComponentBase, IDisposable
    {
        [Inject] private RegistrationCallSuccessViewState RegistrationCallSuccessViewState { get; set; } = null!;

        [Inject] private RegistrationCallSuccessViewService RegistrationCallSuccessViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationCallSuccessViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationCallSuccessViewState);
        }
    }
}
