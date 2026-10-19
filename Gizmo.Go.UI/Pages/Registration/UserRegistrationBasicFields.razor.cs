using System.Linq.Expressions;
using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationBasicFields : ComponentBase, IDisposable
    {
        [Inject]
        private UserRegistrationBasicFieldsViewState UserRegistrationBasicFieldsViewState { get; set; } = null!;

        [Inject]
        private UserRegistrationBasicFieldsViewService UserRegistrationBasicFieldsViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        private bool _showPassword;
        private bool _showConfirm;

        private PasswordTooltipViewState Tooltip => UserRegistrationBasicFieldsViewState.PasswordTooltip;

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationBasicFieldsViewState);
            this.SubscribeChange(UserRegistrationBasicFieldsViewState.PasswordTooltip);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationBasicFieldsViewState);
            this.UnsubscribeChange(UserRegistrationBasicFieldsViewState.PasswordTooltip);
        }

        private void ToggleShowPassword() => _showPassword = !_showPassword;

        private void ToggleShowConfirm() => _showConfirm = !_showConfirm;

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);
            return UserRegistrationBasicFieldsViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private void OnPasswordInput(ChangeEventArgs e) =>
            UserRegistrationBasicFieldsViewService.SetPassword(e.Value?.ToString() ?? string.Empty);

        private void OnRepeatPasswordInput(ChangeEventArgs e) =>
            UserRegistrationBasicFieldsViewService.SetRepeatPassword(e.Value?.ToString() ?? string.Empty);

        private async Task ContinueAsync() => await UserRegistrationBasicFieldsViewService.ContinueToProfileAsync();

        private async Task NavigateBack() => await UserRegistrationBasicFieldsViewService.NavigateBackAsync();
    }
}
