using Gizmo.Go.UI.View.States;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages
{
    [Register()]
    [Route("/create-account")]
    public sealed class CreateAccountViewService : ViewStateServiceBase<CreateAccountViewState>
    {
        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;

        public CreateAccountViewService(
            CreateAccountViewState viewState,
            ILogger<CreateAccountViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
        }

        #endregion

        #region METHODS

        public ValueTask SetPhoneAsync(string raw)
        {
            ViewState.Phone = new string(raw.Where(char.IsDigit).Take(10).ToArray());
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask ToggleTermsAsync()
        {
            ViewState.TermsAccepted = !ViewState.TermsAccepted;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask SubmitAsync()
        {
            if (!ViewState.IsFormValid)
                return ValueTask.CompletedTask;

            var encodedPhone = Uri.EscapeDataString("+7" + ViewState.Phone);
            _navigationService.NavigateTo($"/confirmation?phone={encodedPhone}");
            return ValueTask.CompletedTask;
        }

        public ValueTask NavigateBackAsync()
        {
            _navigationService.NavigateTo("/login");
            return ValueTask.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ViewState.Phone = string.Empty;
            ViewState.TermsAccepted = false;
            ViewState.RaiseChanged();
            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        #endregion
    }
}
