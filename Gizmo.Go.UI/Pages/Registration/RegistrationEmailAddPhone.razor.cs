using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationEmailAddPhone : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject] private RegistrationEmailAddPhoneViewState RegistrationEmailAddPhoneViewState { get; set; } = null!;

        [Inject] private RegistrationEmailAddPhoneViewService RegistrationEmailAddPhoneViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationEmailAddPhoneViewState);
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        private bool CanSubmit => RegistrationEmailAddPhoneViewState.Phone.Length >= 7;

        private async Task OnPhoneInput(ChangeEventArgs e) =>
            await RegistrationEmailAddPhoneViewService.SetPhoneAsync(e.Value?.ToString() ?? string.Empty);

        private async Task SubmitAsync() =>
            await RegistrationEmailAddPhoneViewService.SubmitAsync();

        private async Task NavigateBack() =>
            await RegistrationEmailAddPhoneViewService.NavigateBackAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationEmailAddPhoneViewState);
        }

        #endregion
    }
}
