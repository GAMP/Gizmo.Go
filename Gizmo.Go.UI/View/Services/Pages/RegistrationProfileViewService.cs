using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages
{
    [Register()]
    [Route(NavigationHelper.RegistrationProfilePage)]
    public sealed class RegistrationProfileViewService : ValidatingViewStateServiceBase<RegistrationProfileViewState>
    {
        #region CONSTRUCTOR

        private readonly NavigationService _navigationService;
        private readonly IRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;

        public RegistrationProfileViewService(
            RegistrationProfileViewState viewState,
            ILogger<RegistrationProfileViewService> logger,
            IServiceProvider serviceProvider,
            NavigationService navigationService,
            IRegistrationService registrationService,
            IRegistrationSessionService registrationSession) : base(viewState, logger, serviceProvider)
        {
            _navigationService = navigationService;
            _registrationService = registrationService;
            _registrationSession = registrationSession;
        }

        #endregion

        #region METHODS

        public ValueTask SetUsernameAsync(string value)
        {
            ViewState.Username = value;
            ValidateProperty(() => ViewState.Username);
            return ValueTask.CompletedTask;
        }

        public ValueTask SetEmailAsync(string value)
        {
            ViewState.Email = value;
            ViewState.RaiseChanged();
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

        public ValueTask SetAddressAsync(string value)
        {
            ViewState.Address = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask SetCityAsync(string value)
        {
            ViewState.City = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask SetCountryAsync(string value)
        {
            ViewState.Country = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask SetPostCodeAsync(string value)
        {
            ViewState.PostCode = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask SetPhoneAsync(string value)
        {
            ViewState.Phone = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask SetMobilePhoneAsync(string value)
        {
            ViewState.MobilePhone = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public ValueTask SetSexAsync(UserSex value)
        {
            ViewState.Sex = value;
            ViewState.RaiseChanged();
            return ValueTask.CompletedTask;
        }

        public async ValueTask SubmitAsync(CancellationToken cancellationToken = default)
        {
            Validate();
            if (ViewState.IsValid != true || ViewState.IsSubmitting)
                return;

            ViewState.IsSubmitting = true;
            ViewState.ErrorCode = null;
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
                ViewState.ErrorCode = RegistrationCompleteResultCode.Unknown;
                ViewState.IsSubmitting = false;
                ViewState.RaiseChanged();
                return;
            }

            if (result.Result == RegistrationCompleteResultCode.Success)
            {
                _registrationSession.Clear();
                _navigationService.NavigateTo(NavigationHelper.WelcomePage);
                return;
            }

            ViewState.ErrorCode = result.Result;
            ViewState.IsSubmitting = false;
            ViewState.RaiseChanged();
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (!_registrationSession.HasSession)
            {
                _navigationService.NavigateTo(NavigationHelper.RegistrationPhone);
                return base.OnNavigatedIn(navigationParameters, cancellationToken);
            }

            ViewState.Username = string.Empty;
            ViewState.Email = string.Empty;
            ViewState.FirstName = string.Empty;
            ViewState.LastName = string.Empty;
            ViewState.BirthDate = null;
            ViewState.Address = string.Empty;
            ViewState.City = string.Empty;
            ViewState.Country = string.Empty;
            ViewState.PostCode = string.Empty;
            ViewState.Phone = string.Empty;
            ViewState.MobilePhone = string.Empty;
            ViewState.Sex = UserSex.Unspecified;
            ViewState.IsSubmitting = false;
            ViewState.ErrorCode = null;
            ViewState.RaiseChanged();

            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        #endregion

        #region PRIVATE METHODS

        private RegistrationProfile BuildProfile()
        {
            return new RegistrationProfile
            {
                Username = ViewState.Username,
                Email = NullIfEmpty(ViewState.Email),
                FirstName = NullIfEmpty(ViewState.FirstName),
                LastName = NullIfEmpty(ViewState.LastName),
                BirthDate = ViewState.BirthDate,
                Address = NullIfEmpty(ViewState.Address),
                City = NullIfEmpty(ViewState.City),
                Country = NullIfEmpty(ViewState.Country),
                PostCode = NullIfEmpty(ViewState.PostCode),
                Phone = NullIfEmpty(ViewState.Phone),
                MobilePhone = NullIfEmpty(ViewState.MobilePhone),
                Sex = ViewState.Sex,
            };
        }

        private static string? NullIfEmpty(string s) =>
            string.IsNullOrEmpty(s) ? null : s;

        #endregion
    }
}
