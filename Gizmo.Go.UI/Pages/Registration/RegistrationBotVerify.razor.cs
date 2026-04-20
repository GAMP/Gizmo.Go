using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration;

public partial class RegistrationBotVerify : ComponentBase, IDisposable
{
    #region PROPERTIES

    [Inject] private RegistrationBotVerifyViewState RegistrationBotVerifyViewState { get; set; } = null!;

    [Inject] private RegistrationBotVerifyViewService RegistrationBotVerifyViewService { get; set; } = null!;

    [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

    #endregion

    #region OVERRIDES

    protected override void OnInitialized()
    {
        this.SubscribeChange(RegistrationBotVerifyViewState);
        base.OnInitialized();
    }

    #endregion

    #region METHODS

    private async Task NavigateBack() => await RegistrationBotVerifyViewService.NavigateBackAsync();

    private async Task ConfirmAsync() => await RegistrationBotVerifyViewService.ConfirmAsync();

    public void Dispose()
    {
        this.UnsubscribeChange(RegistrationBotVerifyViewState);
    }

    #endregion
}
