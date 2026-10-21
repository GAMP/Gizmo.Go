using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Common;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationSuccessRoute)]
    public sealed class UserRegistrationSuccessViewService : ViewStateServiceBase<UserRegistrationSuccessViewState>
    {
        private readonly IRegistrationSessionService _registrationSession;
        private readonly IPhoneValidationService _phoneValidationService;

        public UserRegistrationSuccessViewService(
            UserRegistrationSuccessViewState viewState,
            ILogger<UserRegistrationSuccessViewService> logger,
            IServiceProvider serviceProvider,
            IRegistrationSessionService registrationSession,
            IPhoneValidationService phoneValidationService) : base(viewState, logger, serviceProvider)
        {
            _registrationSession = registrationSession;
            _phoneValidationService = phoneValidationService;
        }

        public Task NavigateToLoginAsync()
        {
            _registrationSession.Clear();
            NavigationService.NavigateTo(NavigationHelper.LoginPage);
            return Task.CompletedTask;
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            var contact = _registrationSession.CompletedContact;
            var isEmail = _registrationSession.IsCompletedContactEmail;

            // shown once: a later visit of the route must not reveal the contact of a previous registration
            _registrationSession.SetCompletedContact(null, false);

            ViewState.IsEmailContact = isEmail;
            ViewState.DisplayContact = string.IsNullOrEmpty(contact)
                ? string.Empty
                : isEmail
                    ? ContactMasking.MaskEmail(contact)
                    : ContactMasking.MaskPhone(contact, await _phoneValidationService.GetCountriesAsync(cancellationToken));
            ViewState.RaiseChanged();

            await base.OnNavigatedIn(navigationParameters, cancellationToken);
        }
    }
}
