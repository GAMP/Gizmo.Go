using Gizmo.Client.UI;
using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.Services.Dialog;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages.Registration
{
    [Register()]
    [Route(NavigationHelper.RegistrationIndexRoute)]
    public sealed class UserRegistrationIndexViewService : ViewStateServiceBase<UserRegistrationIndexViewState>
    {
        public UserRegistrationIndexViewService(UserRegistrationIndexViewState viewState,
            ILogger<UserRegistrationIndexViewService> logger,
            IServiceProvider serviceProvider,
            IUserRegistrationService registrationService,
            IGizmoGoDialogService dialogService,
            IRegistrationSessionService registrationSession,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _registrationService = registrationService;
            _dialogService = dialogService;
            _registrationSession = registrationSession;
            _localizationService = localizationService;
        }

        private readonly IUserRegistrationService _registrationService;
        private readonly IGizmoGoDialogService _dialogService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly ILocalizationService _localizationService;

        // mobile deviation: the page shows the load error with a way back instead of an endless loading state
        public Task NavigateBackAsync()
        {
            NavigationService.NavigateTo(NavigationHelper.WelcomePage);
            return Task.CompletedTask;
        }

        public async Task<bool> ProcessUserAgreements(IReadOnlyList<RegistrationAgreement> agreements, CancellationToken cancellationToken = default)
        {
            if (!agreements.Any())
            {
                ViewState.UserAgreementStates = Enumerable.Empty<UserAgreementViewState>();
                _registrationSession.SetAgreementsAccepted(true);
                _registrationSession.SetAgreementChoices(Enumerable.Empty<RegistrationAgreementChoice>());
                return true;
            }

            var userAgreementStates = agreements.Select(a => new UserAgreementViewState()
            {
                Id = a.Id,
                Name = a.Name,
                Agreement = a.Agreement,
                IsRejectable = a.IsRejectable,
                AcceptState = UserAgreementAcceptState.None
            }).ToList();

            foreach (var userAgreement in userAgreementStates)
            {
                var addDialogResult = await _dialogService.ShowUserAgreementDialogAsync(new UserAgreementDialogParameters()
                {
                    Name = userAgreement.Name ?? string.Empty,
                    Agreement = userAgreement.Agreement ?? string.Empty,
                    IsRejectable = userAgreement.IsRejectable
                },
                cancellationToken: cancellationToken);

                if (addDialogResult.Result == AddComponentResultCode.Opened)
                {
                    var dialogResult = await addDialogResult.WaitForResultAsync(cancellationToken);
                    if (addDialogResult.Result == AddComponentResultCode.Ok)
                    {
                        if (dialogResult?.Accepted == true)
                        {
                            userAgreement.AcceptState = UserAgreementAcceptState.Accepted;
                        }
                        else if (userAgreement.IsRejectable)
                        {
                            userAgreement.AcceptState = UserAgreementAcceptState.Rejected;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (addDialogResult.Result == AddComponentResultCode.Dismissed)
                    {
                        if (userAgreement.IsRejectable)
                        {
                            userAgreement.AcceptState = UserAgreementAcceptState.Rejected;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (addDialogResult.Result == AddComponentResultCode.Canceled)
                    {
                        return false;
                    }
                }
            }

            ViewState.UserAgreementStates = userAgreementStates;
            _registrationSession.SetAgreementsAccepted(true);
            _registrationSession.SetAgreementChoices(
                userAgreementStates.Select(a => new RegistrationAgreementChoice
                {
                    AgreementId = a.Id,
                    AcceptState = a.AcceptState
                }));

            return true;
        }

        public void ClearAll()
        {
            _registrationSession.Clear();
            ViewState.UserAgreementStates = Enumerable.Empty<UserAgreementViewState>();
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            DebounceViewStateChanged();
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ClearAll();

            try
            {
                // mobile deviation: direct registration is not supported, the provider flow is the only one
                var providers = await _registrationService.GetMethodsAsync(cancellationToken);
                if (providers.Count == 0)
                {
                    NavigationService.NavigateTo(NavigationHelper.RegistrationErrorRoute);
                    return;
                }

                var agreements = await _registrationService.GetAgreementsAsync(cancellationToken);

                var agreementStatus = await ProcessUserAgreements(agreements, cancellationToken);

                if (agreementStatus)
                {
                    var userGroupDefaultRequiredInfo = await _registrationService.GetRequiredUserInfoAsync(cancellationToken);

                    _registrationSession.SetRequiredUserInfo(userGroupDefaultRequiredInfo);

                    NavigationService.NavigateTo(NavigationHelper.RegistrationProvidersRoute);
                }
                else
                {
                    NavigationService.NavigateTo(NavigationHelper.LoginPage);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Registration service error.");

                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Go.UI.Autogenerated.Resources.GO_GEN_AN_ERROR_HAS_OCCURRED));
                ViewState.RaiseChanged();
            }
        }
    }
}
