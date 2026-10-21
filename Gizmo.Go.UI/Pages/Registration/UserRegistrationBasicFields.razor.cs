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
        private string _usernameInputValue = string.Empty;
        private CancellationTokenSource? _usernameDebounceCts;

        private PasswordTooltipViewState Tooltip => UserRegistrationBasicFieldsViewState.PasswordTooltip;

        protected override void OnInitialized()
        {
            _usernameInputValue = UserRegistrationBasicFieldsViewState.Username;
            this.SubscribeChange(UserRegistrationBasicFieldsViewState);
            this.SubscribeChange(UserRegistrationBasicFieldsViewState.PasswordTooltip);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationBasicFieldsViewState);
            _usernameDebounceCts?.Cancel();
            _usernameDebounceCts?.Dispose();
            this.UnsubscribeChange(UserRegistrationBasicFieldsViewState.PasswordTooltip);
        }

        private void ToggleShowPassword() => _showPassword = !_showPassword;

        private void ToggleShowConfirm() => _showConfirm = !_showConfirm;

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);
            return UserRegistrationBasicFieldsViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private async Task OnUsernameInput(ChangeEventArgs e)
        {
            _usernameDebounceCts?.Cancel();
            _usernameDebounceCts = new CancellationTokenSource();

            _usernameInputValue = e.Value?.ToString() ?? string.Empty;

            try
            {
                await Task.Delay(300, _usernameDebounceCts.Token);
                UserRegistrationBasicFieldsViewService.SetUsername(_usernameInputValue);
            }
            catch (OperationCanceledException) { }
        }

        private void OnPasswordInput(ChangeEventArgs e) =>
            UserRegistrationBasicFieldsViewService.SetPassword(e.Value?.ToString() ?? string.Empty);

        private void OnRepeatPasswordInput(ChangeEventArgs e) =>
            UserRegistrationBasicFieldsViewService.SetRepeatPassword(e.Value?.ToString() ?? string.Empty);

        // a username edit that is not applied yet (debounce) keeps the submit button disabled,
        // so a click is never accepted for a stale availability check
        private bool IsUsernamePending => _usernameInputValue != UserRegistrationBasicFieldsViewState.Username;

        // password and confirmation are validated while typing, so their messages are current here
        private bool IsContinueDisabled =>
            IsUsernamePending
            || !UserRegistrationBasicFieldsViewState.IsUsernameAvailable
            || string.IsNullOrEmpty(UserRegistrationBasicFieldsViewState.Password)
            || string.IsNullOrEmpty(UserRegistrationBasicFieldsViewState.RepeatPassword)
            || HasErrors(() => UserRegistrationBasicFieldsViewState.Password)
            || HasErrors(() => UserRegistrationBasicFieldsViewState.RepeatPassword);

        private async Task ContinueAsync() => await UserRegistrationBasicFieldsViewService.ContinueToProfileAsync();

        private async Task NavigateBack() => await UserRegistrationBasicFieldsViewService.NavigateBackAsync();
    }
}
