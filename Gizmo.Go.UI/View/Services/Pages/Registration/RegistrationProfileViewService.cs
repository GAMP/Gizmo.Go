using System.Text.RegularExpressions;
using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationProfilePage)]
    public sealed class RegistrationProfileViewService : ValidatingViewStateServiceBase<RegistrationProfileViewState>
    {
        private readonly NavigationService _navigationService;
        private readonly IRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly ILocalizationService _localizationService;
        private readonly IUserService _userService;

        public RegistrationProfileViewService(
            RegistrationProfileViewState viewState,
            ILogger<RegistrationProfileViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IRegistrationService registrationService,
            IRegistrationSessionService registrationSession,
            ILocalizationService localizationService,
            IUserService userService) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _registrationService = registrationService;
            _registrationSession = registrationSession;
            _localizationService = localizationService;
            _userService = userService;
        }

        public ValueTask SetUsernameAsync(string value)
        {
            ViewState.Username = value;
            ValidateProperty(() => ViewState.Username);
            return ValueTask.CompletedTask;
        }

        public ValueTask SetFirstNameAsync(string value)
        {
            ViewState.FirstName = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask SetLastNameAsync(string value)
        {
            ViewState.LastName = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask SetBirthDateAsync(DateTime? value)
        {
            ViewState.BirthDate = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public async ValueTask SubmitAsync(CancellationToken cancellationToken = default)
        {
            Validate();
            if (ViewState.IsValid != true || ViewState.IsSubmitting)
                return;

            ViewState.IsSubmitting = true;
            ViewState.ErrorMessage = null;
            ViewState.RaiseChanged();

            RegistrationCompleteResult result;
            try
            {
                result = await _registrationService.CompleteAsync(new RegistrationCompleteRequest
                {
                    Token    = _registrationSession.Token,
                    Password = _registrationSession.Password,
                    Profile  = BuildProfile()
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Registration complete request failed.");
                ViewState.IsSubmitting = false;
                ViewState.RaiseChanged();
                _registrationSession.Clear();
                _navigationService.NavigateTo(NavigationHelper.RegistrationProviders + "?error=1");
                return;
            }

            if (result.Result == RegistrationCompleteResultCode.Success)
            {
                _registrationSession.Clear();
                _navigationService.NavigateTo(NavigationHelper.RegistrationSuccess);
                return;
            }

            _registrationSession.Clear();
            _navigationService.NavigateTo(NavigationHelper.RegistrationProviders + "?error=1");
        }

        public ValueTask NavigateBackAsync()
        {
            _navigationService.NavigateTo(NavigationHelper.RegistrationPasswordPage);
            return ValueTask.CompletedTask;
        }

        private static readonly Regex UsernameRegex = new("^[a-zA-Z0-9_-]+$", RegexOptions.Compiled);

        protected override async Task<IEnumerable<string>> OnValidateAsync(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger, CancellationToken cancellationToken)
        {
            if (fieldIdentifier.FieldName != nameof(RegistrationProfileViewState.Username))
                return Enumerable.Empty<string>();

            var value = ViewState.Username;
            if (string.IsNullOrWhiteSpace(value))
                return Enumerable.Empty<string>();

            if (value.Length < 3 || value.Length > 32)
                return new[] { _localizationService.GetString(nameof(Autogenerated.Resources.GO_VE_USERNAME_TOO_SHORT)) };

            if (!UsernameRegex.IsMatch(value))
                return new[] { _localizationService.GetString(nameof(Autogenerated.Resources.GO_VE_USERNAME_INVALID_CHARS)) };

            try
            {
                var exists = await _userService.UsernameExistsAsync(value, cancellationToken);
                if (exists)
                    return new[] { _localizationService.GetString(nameof(Autogenerated.Resources.GO_REGISTRATION_PROFILE_USERNAME_TAKEN)) };
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Username existence check failed (fail-open).");
            }

            return Enumerable.Empty<string>();
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (!_registrationSession.HasSession)
            {
                _navigationService.NavigateTo(NavigationHelper.RegistrationPhone);
                return base.OnNavigatedIn(navigationParameters, cancellationToken);
            }

            ViewState.Username = string.Empty;
            ViewState.FirstName = string.Empty;
            ViewState.LastName = string.Empty;
            ViewState.BirthDate = null;
            ViewState.IsSubmitting = false;
            ViewState.ErrorMessage = null;

            ClearError(() => ViewState.Username);

            ViewState.RaiseChanged();

            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        private RegistrationProfile BuildProfile()
        {
            return new RegistrationProfile
            {
                Username = ViewState.Username,
                FirstName = NullIfEmpty(ViewState.FirstName),
                LastName = NullIfEmpty(ViewState.LastName),
                BirthDate = ViewState.BirthDate,
            };
        }

        private static string? NullIfEmpty(string s) =>
            string.IsNullOrEmpty(s) ? null : s;
    }
}
