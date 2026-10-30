using System.Linq.Expressions;
using Gizmo.Go.UI.View.Services;
using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Go.UI.Pages
{
    public partial class Login : ComponentBase, IDisposable
    {
        [Inject]
        private LoginViewState LoginViewState { get; set; } = null!;

        [Inject]
        private LoginViewService LoginViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        private bool _isPhoneTab = true;
        private bool _showPassword;

        protected override void OnInitialized()
        {
            this.SubscribeChange(LoginViewState);
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoginViewService.LoadOptionsAsync();
            await base.OnInitializedAsync();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(LoginViewState);
        }

        private bool IsPhoneTab => _isPhoneTab;
        private bool ShowPassword => _showPassword;

        private Task RetryOptionsAsync() => LoginViewService.LoadOptionsAsync();

        private void SetPhoneTab()
        {
            if (_isPhoneTab)
                return;

            _isPhoneTab = true;
            LoginViewService.ClearIdentifier();
        }

        private void SetNicknameTab()
        {
            if (!_isPhoneTab)
                return;

            _isPhoneTab = false;
            LoginViewService.ClearIdentifier();
        }

        private Task OnPhoneRegionCodeChanged(string regionCode)
        {
            LoginViewService.SetPhoneRegionCode(regionCode);
            return Task.CompletedTask;
        }

        private async Task OnPhoneValueChanged(string value) => await LoginViewService.SetPhoneAsync(value);

        private void TogglePasswordVisibility() => _showPassword = !_showPassword;

        private async Task OnKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
                await SubmitAsync();
        }

        private async Task SubmitAsync() => await LoginViewService.SubmitAsync();

        private async Task NavigateToCreateAccount() => await LoginViewService.NavigateToCreateAccountAsync();

        private async Task NavigateToPasswordRecovery() => await LoginViewService.NavigateToPasswordRecoveryAsync();

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);
            return LoginViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }
    }
}
