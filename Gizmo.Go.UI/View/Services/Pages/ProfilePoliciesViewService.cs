using Gizmo.Go.Core.Models.Agreements;
using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages
{
    /// <summary>
    /// Policies screen: the enabled club user agreements.
    /// </summary>
    [Register()]
    [Route(NavigationHelper.ProfilePoliciesRoute)]
    public sealed class ProfilePoliciesViewService : ViewStateServiceBase<ProfilePoliciesViewState>
    {
        private readonly IUserAgreementsService _userAgreementsService;
        private readonly NavigationService _navigationService;

        private CancellationTokenSource? _loadCts;

        public ProfilePoliciesViewService(
            ProfilePoliciesViewState viewState,
            ILogger<ProfilePoliciesViewService> logger,
            IServiceProvider serviceProvider,
            IUserAgreementsService userAgreementsService,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _userAgreementsService = userAgreementsService;
            _navigationService = navigationService;
        }

        /// <summary>
        /// Loads the agreements when the screen was opened without a navigation event (a direct url).
        /// </summary>
        public Task EnsureLoadedAsync() => _loadCts is null ? LoadAsync() : Task.CompletedTask;

        public Task LoadAsync()
        {
            CancelLoad();

            var cts = _loadCts = new CancellationTokenSource();
            ViewState.Agreements.Reset();
            RaiseViewStateChanged();

            return ViewState.Agreements.LoadAsync(
                async ct => (IReadOnlyList<UserAgreement>)await _userAgreementsService.GetAsync(ct),
                Logger, "policies", RaiseViewStateChanged, cts.Token);
        }

        public void OpenPolicy(int id) => _navigationService.NavigateTo(NavigationHelper.GetProfilePolicyRoute(id), NavigationHelper.InAppNavigation);

        /// <summary>
        /// Returns to the previous screen; an entry opened directly (without an app history) opens the profile.
        /// </summary>
        /// <param name="historyEntryState">History entry state of the current screen.</param>
        public Task BackAsync(string? historyEntryState) => _navigationService.GoBackOrNavigateAsync(historyEntryState, NavigationHelper.ProfileRoute);

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default) => LoadAsync();

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            CancelLoad();
            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        private void CancelLoad()
        {
            _loadCts?.Cancel();
            _loadCts = null;
        }
    }
}
