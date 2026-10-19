using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration;

public partial class UserRegistrationProviders : ComponentBase, IDisposable
{
    [Inject] private UserRegistrationProvidersViewState UserRegistrationProvidersViewState { get; set; } = null!;

    [Inject] private UserRegistrationProvidersViewService UserRegistrationProvidersViewService { get; set; } = null!;

    [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

    protected override void OnInitialized()
    {
        this.SubscribeChange(UserRegistrationProvidersViewState);
        base.OnInitialized();
    }

    private async Task SelectProviders(int methodId)
    {
        await UserRegistrationProvidersViewService.SelectProviderAsync(methodId);
    }

    private async Task NavigateBack() => await UserRegistrationProvidersViewService.NavigateBackAsync();

    private async Task NavigateToLogin() => await UserRegistrationProvidersViewService.NavigateToLoginAsync();

    public void Dispose()
    {
        this.UnsubscribeChange(UserRegistrationProvidersViewState);
    }
}
