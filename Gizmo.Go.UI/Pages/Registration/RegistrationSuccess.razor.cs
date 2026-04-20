using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationSuccess : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject] private RegistrationSuccessViewState RegistrationSuccessViewState { get; set; } = null!;

        [Inject] private RegistrationSuccessViewService RegistrationSuccessViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationSuccessViewState);
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        private async Task NavigateHomeAsync() =>
            await RegistrationSuccessViewService.NavigateHomeAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationSuccessViewState);
        }

        #endregion
    }
}
