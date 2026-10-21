using System.Linq.Expressions;
using Gizmo.Go.UI.View.Services.Pages.PasswordRecovery;
using Gizmo.Go.UI.View.States;
using Gizmo.Go.UI.View.States.Pages.PasswordRecovery;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Gizmo.Go.UI.Pages.PasswordRecovery
{
    public partial class PasswordRecoverySetNewPassword : ComponentBase, IDisposable
    {
        [Inject]
        private PasswordRecoverySetNewPasswordViewState PasswordRecoverySetNewPasswordViewState { get; set; } = null!;

        [Inject]
        private PasswordRecoverySetNewPasswordViewService PasswordRecoverySetNewPasswordViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        private bool _showPassword;
        private bool _showConfirm;

        private PasswordTooltipViewState Tooltip => PasswordRecoverySetNewPasswordViewState.PasswordTooltip;

        // both fields are validated while typing, so their messages are current here
        private bool IsSubmitDisabled =>
            PasswordRecoverySetNewPasswordViewState.IsLoading
            || string.IsNullOrEmpty(PasswordRecoverySetNewPasswordViewState.NewPassword)
            || string.IsNullOrEmpty(PasswordRecoverySetNewPasswordViewState.RepeatPassword)
            || HasErrors(() => PasswordRecoverySetNewPasswordViewState.NewPassword)
            || HasErrors(() => PasswordRecoverySetNewPasswordViewState.RepeatPassword);

        protected override void OnInitialized()
        {
            this.SubscribeChange(PasswordRecoverySetNewPasswordViewState);
            this.SubscribeChange(PasswordRecoverySetNewPasswordViewState.PasswordTooltip);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(PasswordRecoverySetNewPasswordViewState);
            this.UnsubscribeChange(PasswordRecoverySetNewPasswordViewState.PasswordTooltip);
        }

        private void ToggleShowPassword() => _showPassword = !_showPassword;

        private void ToggleShowConfirm() => _showConfirm = !_showConfirm;

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);
            return PasswordRecoverySetNewPasswordViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private void OnNewPasswordInput(ChangeEventArgs e) =>
            PasswordRecoverySetNewPasswordViewService.SetNewPassword(e.Value?.ToString() ?? string.Empty);

        private void OnRepeatPasswordInput(ChangeEventArgs e) =>
            PasswordRecoverySetNewPasswordViewService.SetRepeatPassword(e.Value?.ToString() ?? string.Empty);

        private async Task SubmitAsync() => await PasswordRecoverySetNewPasswordViewService.SubmitAsync();

        private async Task NavigateBack() => await PasswordRecoverySetNewPasswordViewService.NavigateBackAsync();
    }
}
