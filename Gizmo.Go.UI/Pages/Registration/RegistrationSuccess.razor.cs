using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationSuccess : ComponentBase, IDisposable
    {
        [Inject] private RegistrationSuccessViewState RegistrationSuccessViewState { get; set; } = null!;

        [Inject] private RegistrationSuccessViewService RegistrationSuccessViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationSuccessViewState);
            base.OnInitialized();
        }

        private async Task NavigateHomeAsync() =>
            await RegistrationSuccessViewService.NavigateHomeAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationSuccessViewState);
        }
    }
}
