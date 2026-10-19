using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationCallVerify : ComponentBase, IDisposable
    {
        [Inject]
        private UserRegistrationCallVerifyViewState UserRegistrationCallVerifyViewState { get; set; } = null!;

        [Inject]
        private UserRegistrationCallVerifyViewService UserRegistrationCallVerifyViewService { get; set; } = null!;

        [Inject]
        private UserRegistrationCallPhoneViewState UserRegistrationCallPhoneViewState { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationCallVerifyViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationCallVerifyViewState);
        }

        private async Task CopyToClipboardAsync()
        {
            if (string.IsNullOrEmpty(UserRegistrationCallVerifyViewState.ServerPhoneNumber))
                return;

            try
            {
                await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText",
                    UserRegistrationCallVerifyViewState.ServerPhoneNumber);
            }
            catch (JSException) { }
            catch (JSDisconnectedException) { }
        }

        private async Task CancelAsync() => await UserRegistrationCallVerifyViewService.CancelAsync();
    }
}
