using Gizmo.Go.UI.View.Services;
using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationPassword : ComponentBase, IDisposable
    {
        [Inject]
        private RegistrationPasswordViewState RegistrationPasswordViewState { get; set; } = null!;

        [Inject]
        private RegistrationPasswordViewService RegistrationPasswordViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        private bool _showPassword;
        private bool _showConfirm;

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationPasswordViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationPasswordViewState);
        }

        private void ToggleShowPassword() => _showPassword = !_showPassword;

        private void ToggleShowConfirm() => _showConfirm = !_showConfirm;

        private async Task OnPasswordInput(ChangeEventArgs e) =>
            await RegistrationPasswordViewService.SetPasswordAsync(e.Value?.ToString() ?? string.Empty);

        private async Task OnConfirmInput(ChangeEventArgs e) =>
            await RegistrationPasswordViewService.SetConfirmAsync(e.Value?.ToString() ?? string.Empty);

        private async Task SubmitAsync() => await RegistrationPasswordViewService.SubmitAsync();

        private async Task NavigateBack() => await RegistrationPasswordViewService.NavigateBackAsync();
    }
}
