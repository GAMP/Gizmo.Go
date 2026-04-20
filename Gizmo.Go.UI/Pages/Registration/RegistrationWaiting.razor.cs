using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationWaiting : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject] private RegistrationWaitingViewState RegistrationWaitingViewState { get; set; } = null!;

        [Inject] private RegistrationWaitingViewService RegistrationWaitingViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationWaitingViewState);
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        private async Task NavigateBack() =>
            await RegistrationWaitingViewService.NavigateBackAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationWaitingViewState);
        }

        #endregion
    }
}
