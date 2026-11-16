using System.Globalization;
using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages
{
    public partial class Booking : ComponentBase, IDisposable
    {
        private int _loadedId = -1;

        [Inject]
        private BookingViewState BookingViewState { get; set; } = null!;

        [Inject]
        private BookingViewService BookingViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the step of the flow (1 session, 2 PC selection, 3 confirmation). The value is a string parsed in
        /// <see cref="OnParametersSetAsync"/>: an invalid value shows step 1 instead of failing the page.
        /// </summary>
        [SupplyParameterFromQuery(Name = "step")]
        public string? StepText { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(BookingViewState);
            base.OnInitialized();
        }

        // the view service has no route (a route with a parameter never matches its navigation events), the page starts the
        // load with the id from the route; a navigation that only changes the step keeps the page and the flow state
        protected override async Task OnParametersSetAsync()
        {
            if (_loadedId != Id)
            {
                _loadedId = Id;
                await BookingViewService.LoadAsync(Id);
            }

            await ApplyStepAsync();
            await base.OnParametersSetAsync();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(BookingViewState);
            BookingViewService.CancelLoad();
        }

        private async Task ReloadAsync()
        {
            await BookingViewService.LoadAsync(Id);
            await ApplyStepAsync();
        }

        private Task ApplyStepAsync()
        {
            var step = int.TryParse(StepText, NumberStyles.None, CultureInfo.InvariantCulture, out var parsed) ? parsed : 1;
            return BookingViewService.ApplyStepAsync(Id, step, NavigationManager.HistoryEntryState);
        }

        private Task BackAsync() => BookingViewService.BackAsync(NavigationManager.HistoryEntryState);
    }
}
