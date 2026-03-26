using Gizmo.Go.UI.View.Services;
using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages
{
    public partial class CreatePassword : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        private CreatePasswordViewState CreatePasswordViewState { get; set; } = null!;

        [Inject]
        private CreatePasswordViewService CreatePasswordViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region FIELDS

        private bool _showPassword;
        private bool _showConfirm;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(CreatePasswordViewState);
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        public void Dispose()
        {
            this.UnsubscribeChange(CreatePasswordViewState);
        }

        private void ToggleShowPassword() => _showPassword = !_showPassword;

        private void ToggleShowConfirm() => _showConfirm = !_showConfirm;

        private async Task OnPasswordInput(ChangeEventArgs e) =>
            await CreatePasswordViewService.SetPasswordAsync(e.Value?.ToString() ?? string.Empty);

        private async Task OnConfirmInput(ChangeEventArgs e) =>
            await CreatePasswordViewService.SetConfirmAsync(e.Value?.ToString() ?? string.Empty);

        private async Task SubmitAsync() => await CreatePasswordViewService.SubmitAsync();

        private async Task NavigateBack() => await CreatePasswordViewService.NavigateBackAsync();

        #endregion
    }
}
