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
        #region PROPERTIES

        [Inject]
        private LoginViewState LoginViewState { get; set; } = null!;

        [Inject]
        private LoginViewService LoginViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region FIELDS

        private bool _isPhoneTab = true;
        private bool _showPassword;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(LoginViewState);
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        public void Dispose()
        {
            this.UnsubscribeChange(LoginViewState);
        }

        private bool IsPhoneTab => _isPhoneTab;
        private bool ShowPassword => _showPassword;

        private void SetPhoneTab() => _isPhoneTab = true;

        private void SetNicknameTab() => _isPhoneTab = false;

        private void TogglePasswordVisibility() => _showPassword = !_showPassword;

        private async Task OnKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
                await SubmitAsync();
        }

        private async Task SubmitAsync() => await LoginViewService.SubmitAsync();

        private async Task NavigateToCreateAccount() => await LoginViewService.NavigateToCreateAccountAsync();

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);
            return LoginViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        #endregion
    }
}
