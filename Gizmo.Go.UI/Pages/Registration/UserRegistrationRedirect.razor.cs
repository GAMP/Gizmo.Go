using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationRedirect : ComponentBase, IDisposable
    {
        [Inject] private UserRegistrationRedirectViewState UserRegistrationRedirectViewState { get; set; } = null!;

        [Inject] private UserRegistrationRedirectViewService UserRegistrationRedirectViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationRedirectViewState);
            base.OnInitialized();
        }

        private async Task NavigateBack() =>
            await UserRegistrationRedirectViewService.NavigateBackAsync();

        private async Task NavigateToProviders() =>
            await UserRegistrationRedirectViewService.NavigateToProvidersAsync();

        private async Task OpenAsync() =>
            await UserRegistrationRedirectViewService.OpenAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationRedirectViewState);
        }
    }
}
