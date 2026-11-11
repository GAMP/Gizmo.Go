using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Helpers;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.View.Services.Pages
{
    /// <summary>
    /// Policy text. The route has a parameter (<see cref="NavigationHelper.ProfilePolicyRoute"/>), which the navigation events of
    /// the view services do not match, so the page starts the load with the id from the route.
    /// </summary>
    [Register()]
    public sealed class ProfilePolicyViewService : ViewStateServiceBase<ProfilePolicyViewState>
    {
        private readonly IUserAgreementsService _userAgreementsService;
        private readonly NavigationService _navigationService;

        private CancellationTokenSource? _loadCts;

        public ProfilePolicyViewService(
            ProfilePolicyViewState viewState,
            ILogger<ProfilePolicyViewService> logger,
            IServiceProvider serviceProvider,
            IUserAgreementsService userAgreementsService,
            NavigationService navigationService) : base(viewState, logger, serviceProvider)
        {
            _userAgreementsService = userAgreementsService;
            _navigationService = navigationService;
        }

        /// <summary>
        /// Loads the agreement by id (the server has no by-id endpoint, the item is taken from the list).
        /// </summary>
        public async Task LoadAsync(int id)
        {
            CancelLoad();

            var cts = _loadCts = new CancellationTokenSource();
            ViewState.Agreement.Reset();
            ViewState.IsNotFound = false;
            RaiseViewStateChanged();

            await ViewState.Agreement.LoadAsync(
                async ct => (await _userAgreementsService.GetAsync(ct)).FirstOrDefault(x => x.Id == id),
                Logger, "policy", RaiseViewStateChanged, cts.Token);

            if (cts.IsCancellationRequested)
                return;

            ViewState.IsNotFound = !ViewState.Agreement.HasError && ViewState.Agreement.Value is null;
            RaiseViewStateChanged();
        }

        /// <summary>
        /// Cancels the running load (the page is left).
        /// </summary>
        public void CancelLoad()
        {
            _loadCts?.Cancel();
            _loadCts = null;
        }

        /// <summary>
        /// Returns to the previous screen; an entry opened directly (without an app history) opens the policies list.
        /// </summary>
        /// <param name="historyEntryState">History entry state of the current screen.</param>
        public Task BackAsync(string? historyEntryState) => _navigationService.GoBackOrNavigateAsync(historyEntryState, NavigationHelper.ProfilePoliciesRoute);
    }
}
