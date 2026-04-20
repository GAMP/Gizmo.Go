using System.Web;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationBotVerify)]
    public sealed class RegistrationBotVerifyViewService : ViewStateServiceBase<RegistrationBotVerifyViewState>
    {
        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;

        public RegistrationBotVerifyViewService(
            RegistrationBotVerifyViewState viewState,
            ILogger<RegistrationBotVerifyViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
        }

        #endregion

        #region METHODS

        public ValueTask ConfirmAsync()
        {
            // TODO: wire confirmation logic
            _navigationService.NavigateTo(NavigationHelper.RegistrationProfilePage);
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateBackAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationProviders);
            return ValueTask.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            var uri = new Uri(_navigationService.GetUri());
            ViewState.DisplayPhone = HttpUtility.ParseQueryString(uri.Query).Get("phone") ?? string.Empty;
            ViewState.RaiseChanged();

            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        #endregion
    }
}
