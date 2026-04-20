using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationEmail : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject] private RegistrationEmailViewState RegistrationEmailViewState { get; set; } = null!;

        [Inject] private RegistrationEmailViewService RegistrationEmailViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationEmailViewState);
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        private bool CanSubmit => RegistrationEmailViewState.Email.Contains('@') && RegistrationEmailViewState.Email.IndexOf('@') > 0;

        private async Task OnEmailInput(ChangeEventArgs e) =>
            await RegistrationEmailViewService.SetEmailAsync(e.Value?.ToString() ?? string.Empty);

        private async Task SubmitAsync() =>
            await RegistrationEmailViewService.SubmitAsync();

        private async Task NavigateBack() =>
            await RegistrationEmailViewService.NavigateBackAsync();

        private async Task NavigateToAlternative() =>
            await RegistrationEmailViewService.NavigateToAlternativeAsync();

        private async Task NavigateToLogin() =>
            await RegistrationEmailViewService.NavigateToLoginAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationEmailViewState);
        }

        #endregion
    }
}
