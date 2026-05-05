using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationCallPhone : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        private RegistrationCallPhoneViewState RegistrationCallPhoneViewState { get; set; } = null!;

        [Inject]
        private RegistrationCallPhoneViewService RegistrationCallPhoneViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationCallPhoneViewState);
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        private async Task SubmitAsync() => await RegistrationCallPhoneViewService.SubmitAsync();

        private async Task NavigateBack() => await RegistrationCallPhoneViewService.NavigateBackAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationCallPhoneViewState);
        }

        #endregion
    }
}
