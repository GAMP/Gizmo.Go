using Gizmo.Go.UI.View.Models;
using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Go.UI.Pages
{
    public partial class CreateAccount : ComponentBase, IAsyncDisposable
    {
        #region PROPERTIES

        [Inject]
        private CreateAccountViewState CreateAccountViewState { get; set; } = null!;

        [Inject]
        private CreateAccountViewService CreateAccountViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        #endregion

        #region FIELDS

        private ElementReference _phoneInput;
        private DotNetObjectReference<CreateAccount>? _dotNetReference;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(CreateAccountViewState);
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            _dotNetReference = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("gizmoCreateAccountPhone.init", _phoneInput, _dotNetReference);
        }

        #endregion

        #region METHODS

        private async Task ToggleTerms() => await CreateAccountViewService.ToggleTermsAsync();

        private void OnTermsKeyDown(KeyboardEventArgs args)
        {
            if (args.Key is " " or "Enter")
                _ = CreateAccountViewService.ToggleTermsAsync();
        }

        private async Task SubmitAsync() => await CreateAccountViewService.SubmitAsync();

        private async Task NavigateToLogin() => await CreateAccountViewService.NavigateBackAsync();

        [JSInvokable]
        public async Task OnPhoneChangedAsync(PhoneInputChangedModel model)
        {
            await CreateAccountViewService.UpdatePhoneAsync(model);
        }

        public async ValueTask DisposeAsync()
        {
            this.UnsubscribeChange(CreateAccountViewState);

            try
            {
                await JSRuntime.InvokeVoidAsync("gizmoCreateAccountPhone.destroy", _phoneInput);
            }
            catch (JSDisconnectedException)
            {
            }

            _dotNetReference?.Dispose();
        }

        #endregion
    }
}
