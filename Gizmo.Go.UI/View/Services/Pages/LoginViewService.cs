using System.Web;
using Gizmo;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services
{
    [Register()]
    [Route("/login")]
    public sealed class LoginViewService : ValidatingViewStateServiceBase<LoginViewState>
    {
        #region CONSTRUCTOR

        private readonly IAuthService _authService;
        private readonly NavigationService _navigationService;
        private readonly IAssemblyResourcesLocalizationService _assemblyResourcesLocalizationService;
        private string? _returnUrl;

        public LoginViewService(
            LoginViewState viewState,
            ILogger<LoginViewService> logger,
            IServiceProvider serviceProvider,
            IAuthService authService,
            NavigationService navigationService,
            IAssemblyResourcesLocalizationService assemblyResourcesLocalizationService) : base(viewState, logger, serviceProvider)
        {
            _authService = authService;
            _navigationService = navigationService;
            _assemblyResourcesLocalizationService = assemblyResourcesLocalizationService;
        }

        #endregion

        #region METHODS

        public ValueTask SetUsernameAsync(string value)
        {
            ViewState.Username = value;
            ViewState.ErrorMessage = null;
            ValidateProperty(() => ViewState.Username);
            return ValueTask.CompletedTask;
        }

        public ValueTask SetPasswordAsync(string value)
        {
            ViewState.Password = value;
            ViewState.ErrorMessage = null;
            ValidateProperty(() => ViewState.Password);
            return ValueTask.CompletedTask;
        }

        public async ValueTask SubmitAsync(CancellationToken cancellationToken = default)
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.ErrorMessage = null;
            ViewState.RaiseChanged();

            try
            {
                var result = await _authService.LoginAsync(ViewState.Username, ViewState.Password, cancellationToken);

                if (result.Success)
                {
                    var target = !string.IsNullOrWhiteSpace(_returnUrl) ? Uri.UnescapeDataString(_returnUrl) : "";
                    _navigationService.NavigateTo(target);
                }
                else if (result.ErrorCode.HasValue)
                {
                    ViewState.ErrorMessage = _assemblyResourcesLocalizationService.GetLocalizedStringValueOrName(result.ErrorCode.Value);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Login failed.");
                ViewState.ErrorMessage = _assemblyResourcesLocalizationService.GetLocalizedStringValueOrName(AuthErrorCode.Unexpected);
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public ValueTask NavigateToCreateAccountAsync()
        {
            _navigationService.NavigateTo("/create-account");
            return ValueTask.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ViewState.Username = string.Empty;
            ViewState.Password = string.Empty;
            ViewState.IsLoading = false;
            ViewState.ErrorMessage = null;

            var currentUri = _navigationService.GetUri();
            if (!string.IsNullOrEmpty(currentUri))
            {
                var uri = new Uri(currentUri);
                _returnUrl = HttpUtility.ParseQueryString(uri.Query).Get("returnUrl");
            }

            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        #endregion
    }
}
