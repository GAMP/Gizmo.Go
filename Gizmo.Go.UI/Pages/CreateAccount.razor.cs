using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Go.UI.Pages
{
    public partial class CreateAccount : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        private CreateAccountViewState CreateAccountViewState { get; set; } = null!;

        [Inject]
        private CreateAccountViewService CreateAccountViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(CreateAccountViewState);
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        public void Dispose()
        {
            this.UnsubscribeChange(CreateAccountViewState);
        }

        private async Task ToggleTerms() => await CreateAccountViewService.ToggleTermsAsync();

        private void OnTermsKeyDown(KeyboardEventArgs args)
        {
            if (args.Key is " " or "Enter")
                _ = CreateAccountViewService.ToggleTermsAsync();
        }

        private async Task OnPhoneInput(ChangeEventArgs e) =>
            await CreateAccountViewService.SetPhoneAsync(e.Value?.ToString() ?? string.Empty);

        private async Task SubmitAsync() => await CreateAccountViewService.SubmitAsync();

        private async Task NavigateToLogin() => await CreateAccountViewService.NavigateBackAsync();

        #endregion
    }
}
