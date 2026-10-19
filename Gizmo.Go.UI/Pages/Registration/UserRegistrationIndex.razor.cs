using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationIndex : ComponentBase, IDisposable
    {
        [Inject]
        private UserRegistrationIndexViewState UserRegistrationIndexViewState { get; set; } = null!;

        // injected so the view service is created and handles the navigation to the route
        [Inject]
        private UserRegistrationIndexViewService UserRegistrationIndexViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationIndexViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationIndexViewState);
        }

        private async Task NavigateBack() => await UserRegistrationIndexViewService.NavigateBackAsync();
    }
}
