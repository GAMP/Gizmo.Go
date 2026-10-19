using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationCallPhone : ComponentBase, IDisposable
    {
        [Inject]
        private UserRegistrationCallPhoneViewState UserRegistrationCallPhoneViewState { get; set; } = null!;

        [Inject]
        private UserRegistrationCallPhoneViewService UserRegistrationCallPhoneViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationCallPhoneViewState);
            base.OnInitialized();
        }

        private async Task SubmitAsync() => await UserRegistrationCallPhoneViewService.SubmitAsync();

        private async Task NavigateBack() => await UserRegistrationCallPhoneViewService.NavigateBackAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationCallPhoneViewState);
        }
    }
}
