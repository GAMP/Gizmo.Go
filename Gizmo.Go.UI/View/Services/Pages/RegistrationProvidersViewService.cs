using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages;

[Register()]
[Route(NavigationHelper.RegistrationProviders)]
public class RegistrationProvidersViewService : ViewStateServiceBase<RegistrationProvidersViewState>
{
    #region CONSTRUCTOR

    private readonly IRegistrationService _registrationService;
    private readonly NavigationService _navigationService;
    
    public RegistrationProvidersViewService(
        RegistrationProvidersViewState viewState,
        ILogger<RegistrationProvidersViewService> logger,
        IServiceProvider serviceProvider,
        IRegistrationService registrationService,
        NavigationService navigationService)
        : base(viewState, logger, serviceProvider)
    {
        _registrationService = registrationService;
        _navigationService = navigationService;
    }
    
    #endregion

    #region METHODS

    public ValueTask SelectProviderAsync(Guid channelGuid)
    {
        var provider = ViewState.Providers.FirstOrDefault(p => p.ChannelGuid == channelGuid);
        if (provider is null)
        {
            return ValueTask.CompletedTask;
        }

        if (provider is { CanDispatchCode: true, CanRedirect: false })
        {
            _navigationService.NavigateTo($"{NavigationHelper.CreateAccount}?provider={provider.ChannelGuid}");
            return ValueTask.CompletedTask;
        }
        
        return ValueTask.CompletedTask;
    }
    
    #endregion

    #region OVERRIDES

    protected override async Task OnNavigatedIn(NavigationParameters navigationParameters,
        CancellationToken cancellationToken = default)
    {
        ViewState.IsLoading = true;
        ViewState.RaiseChanged();

        try
        {
            ViewState.Providers = await _registrationService.GetProvidersAsync(cancellationToken);
        }
        finally
        {
            ViewState.IsLoading = false;
            ViewState.RaiseChanged();
        }
        
        await base.OnNavigatedIn(navigationParameters, cancellationToken);
    }
    
    public ValueTask NavigateBackAsync()
    {
        _navigationService.NavigateTo(NavigationHelper.WelcomePage);
        return ValueTask.CompletedTask;
    }

    #endregion
}