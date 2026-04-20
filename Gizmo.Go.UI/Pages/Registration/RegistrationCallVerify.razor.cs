using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationCallVerify : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        private RegistrationCallVerifyViewState RegistrationCallVerifyViewState { get; set; } = null!;

        [Inject]
        private RegistrationCallVerifyViewService RegistrationCallVerifyViewService { get; set; } = null!;

        [Inject]
        private RegistrationCallPhoneViewState RegistrationCallPhoneViewState { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationCallVerifyViewState);
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationCallVerifyViewState);
        }

        private async Task CopyToClipboardAsync()
        {
            if (string.IsNullOrEmpty(RegistrationCallVerifyViewState.ServerPhoneNumber))
                return;

            try
            {
                await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText",
                    RegistrationCallVerifyViewState.ServerPhoneNumber);
            }
            catch (JSException) { }
            catch (JSDisconnectedException) { }
        }

        private async Task CancelAsync() => await RegistrationCallVerifyViewService.CancelAsync();

        #endregion
    }
}
