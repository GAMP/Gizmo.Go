using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationSuccess : ComponentBase, IDisposable
    {
        [Inject] private UserRegistrationSuccessViewState UserRegistrationSuccessViewState { get; set; } = null!;

        [Inject] private UserRegistrationSuccessViewService UserRegistrationSuccessViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationSuccessViewState);
            base.OnInitialized();
        }

        private async Task NavigateToLoginAsync() =>
            await UserRegistrationSuccessViewService.NavigateToLoginAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationSuccessViewState);
        }
    }
}
