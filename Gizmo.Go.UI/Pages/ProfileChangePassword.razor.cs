using System.Linq.Expressions;
using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Gizmo.Go.UI.Pages
{
    public partial class ProfileChangePassword : ComponentBase, IDisposable
    {
        [Inject]
        private ProfileChangePasswordViewState ProfileChangePasswordViewState { get; set; } = null!;

        [Inject]
        private ProfileChangePasswordViewService ProfileChangePasswordViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        private bool _showCurrent;
        private bool _showPassword;
        private bool _showConfirm;

        private PasswordTooltipViewState Tooltip => ProfileChangePasswordViewState.PasswordTooltip;

        // all fields are validated while typing, so their messages are current here
        private bool IsSubmitDisabled =>
            ProfileChangePasswordViewState.IsLoading
            || string.IsNullOrEmpty(ProfileChangePasswordViewState.CurrentPassword)
            || string.IsNullOrEmpty(ProfileChangePasswordViewState.NewPassword)
            || string.IsNullOrEmpty(ProfileChangePasswordViewState.RepeatPassword)
            || HasErrors(() => ProfileChangePasswordViewState.CurrentPassword)
            || HasErrors(() => ProfileChangePasswordViewState.NewPassword)
            || HasErrors(() => ProfileChangePasswordViewState.RepeatPassword);

        protected override void OnInitialized()
        {
            this.SubscribeChange(ProfileChangePasswordViewState);
            this.SubscribeChange(ProfileChangePasswordViewState.PasswordTooltip);
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await ProfileChangePasswordViewService.EnsurePasswordRulesLoadedAsync();
            await base.OnInitializedAsync();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(ProfileChangePasswordViewState);
            this.UnsubscribeChange(ProfileChangePasswordViewState.PasswordTooltip);
        }

        private void ToggleShowCurrent() => _showCurrent = !_showCurrent;

        private void ToggleShowPassword() => _showPassword = !_showPassword;

        private void ToggleShowConfirm() => _showConfirm = !_showConfirm;

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);
            return ProfileChangePasswordViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private void OnCurrentPasswordInput(ChangeEventArgs e) =>
            ProfileChangePasswordViewService.SetCurrentPassword(e.Value?.ToString() ?? string.Empty);

        private void OnNewPasswordInput(ChangeEventArgs e) =>
            ProfileChangePasswordViewService.SetNewPassword(e.Value?.ToString() ?? string.Empty);

        private void OnRepeatPasswordInput(ChangeEventArgs e) =>
            ProfileChangePasswordViewService.SetRepeatPassword(e.Value?.ToString() ?? string.Empty);

        private Task SubmitAsync() => ProfileChangePasswordViewService.SubmitAsync();

        private Task BackAsync() => ProfileChangePasswordViewService.BackAsync(NavigationManager.HistoryEntryState);

        private Task DoneAsync()
        {
            ProfileChangePasswordViewService.Done();
            return Task.CompletedTask;
        }
    }
}
