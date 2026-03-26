using Gizmo.Go.UI.View.Services;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages
{
    public partial class Welcome : ComponentBase
    {
        #region PROPERTIES

        [Inject]
        private WelcomeViewService WelcomeViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region METHODS

        private async Task NavigateToLogin() => await WelcomeViewService.NavigateToLoginAsync();

        private async Task NavigateToRegister() => await WelcomeViewService.NavigateToRegisterAsync();

        private async Task ContinueAsGuest() => await WelcomeViewService.ContinueAsGuestAsync();

        #endregion
    }
}
