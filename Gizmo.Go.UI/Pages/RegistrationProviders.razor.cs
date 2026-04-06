using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages;

public partial class RegistrationProviders : ComponentBase, IDisposable
{
    #region PROPERTIES

    [Inject] private RegistrationProvidersViewState RegistrationProvidersViewState { get; set; } = null!;

    [Inject] private RegistrationProvidersViewService RegistrationProvidersViewService { get; set; } = null!;

    #endregion

    #region OVERRIDES

    protected override void OnInitialized()
    {
        this.SubscribeChange(RegistrationProvidersViewState);
        base.OnInitialized();
    }

    #endregion

    #region METHODS

    private async Task SelectProviders(Guid channelGuid)
    {
        await RegistrationProvidersViewService.SelectProviderAsync(channelGuid);
    }
    
    private async Task NavigateBack() => await RegistrationProvidersViewService.NavigateBackAsync();

    public void Dispose()
    {
        this.UnsubscribeChange(RegistrationProvidersViewState);
    }

    #endregion
}