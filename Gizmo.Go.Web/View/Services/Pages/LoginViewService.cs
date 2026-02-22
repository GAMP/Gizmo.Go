using System.Web;
using Gizmo;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Web.View.States;
using Gizmo.UI;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.Web.View.Services
{
    [Register()]
    [Route("/login")]
    public sealed class LoginViewService : ValidatingViewStateServiceBase<LoginViewState>
    {
        private readonly IAuthService _authService;
        private readonly NavigationManager _navigationManager;
        private readonly IAssemblyResourcesLocalizationService _assemblyResourcesLocalizationService;
        private string? _returnUrl;

        public LoginViewService(
            LoginViewState viewState,
            ILogger<LoginViewService> logger,
            IServiceProvider serviceProvider,
            IAuthService authService,
            NavigationManager navigationManager,
            IAssemblyResourcesLocalizationService assemblyResourcesLocalizationService) : base(viewState, logger, serviceProvider)
        {
            _authService = authService;
            _navigationManager = navigationManager;
            _assemblyResourcesLocalizationService = assemblyResourcesLocalizationService;
        }

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
                    _navigationManager.NavigateTo(target);
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

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ViewState.Username = string.Empty;
            ViewState.Password = string.Empty;
            ViewState.IsLoading = false;
            ViewState.ErrorMessage = null;

            // capture return URL from query string
            var uri = new Uri(_navigationManager.Uri);
            _returnUrl = HttpUtility.ParseQueryString(uri.Query).Get("returnUrl");

            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }
    }
}
