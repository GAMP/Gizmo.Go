using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationError : ComponentBase
    {
        [Inject]
        private UserRegistrationErrorViewService UserRegistrationErrorViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        private async Task GoBack() => await UserRegistrationErrorViewService.GoBackAsync();

        private async Task TryAgain() => await UserRegistrationErrorViewService.TryAgainAsync();
    }
}
