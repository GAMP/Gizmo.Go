using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationRedirectSuccess : ComponentBase, IDisposable
    {
        [Inject] private UserRegistrationRedirectSuccessViewState UserRegistrationRedirectSuccessViewState { get; set; } = null!;

        [Inject] private UserRegistrationRedirectSuccessViewService UserRegistrationRedirectSuccessViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationRedirectSuccessViewState);
            base.OnInitialized();
        }

        private async Task NavigateBackAsync() => await UserRegistrationRedirectSuccessViewService.NavigateBackAsync();

        private async Task ConfirmAsync() => await UserRegistrationRedirectSuccessViewService.ConfirmAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationRedirectSuccessViewState);
        }
    }
}
