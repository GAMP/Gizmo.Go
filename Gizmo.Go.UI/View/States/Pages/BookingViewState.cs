using Gizmo.Go.Core.Models.Balance;
using Gizmo.Go.Core.Models.Booking;
using Gizmo.Go.Core.Models.Branch;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    /// <summary>
    /// State of the booking flow of one club: the session (step 1), the PC selection (step 2) and the confirmation (step 3).
    /// </summary>
    [Register()]
    public sealed class BookingViewState : ViewStateBase
    {
        /// <summary>
        /// Gets the club (name for the header and the time zone).
        /// </summary>
        public ViewBlock<BranchDetails> Club { get; } = new();

        /// <summary>
        /// Gets the reservation options (duration bounds, fee, minimum payment).
        /// </summary>
        public ViewBlock<BookingOptions> Options { get; } = new();

        /// <summary>
        /// Gets the deposit balance of the user for the payment method.
        /// </summary>
        public ViewBlock<UserBalance> Balance { get; } = new();

        /// <summary>
        /// Gets whether the club does not exist or is not active.
        /// </summary>
        public bool IsNotFound { get; internal set; }

        /// <summary>
        /// Gets the current step: 1 session, 2 PC selection, 3 confirmation.
        /// </summary>
        public int Step { get; internal set; } = 1;

        /// <summary>
        /// Gets whether step 1 was opened inside the app (from the club page); false for a link or a reload.
        /// </summary>
        public bool IsFlowStartedInApp { get; internal set; }

        /// <summary>
        /// Gets the number of history entries of the flow before the screen it was opened from: one entry per step
        /// shown, step 1 counts only when it was opened inside the app (a direct step 1 entry has no screen before it).
        /// </summary>
        public int FlowEntries => Step - 1 + (IsFlowStartedInApp ? 1 : 0);

        /// <summary>
        /// Gets the session durations in minutes the user can choose (the options bounds applied).
        /// </summary>
        public IReadOnlyList<int> Durations { get; internal set; } = [];

        /// <summary>
        /// Gets the chosen date in the club time.
        /// </summary>
        public DateOnly? Date { get; internal set; }

        /// <summary>
        /// Gets the chosen start time in the club time.
        /// </summary>
        public TimeOnly? StartTime { get; internal set; }

        /// <summary>
        /// Gets the chosen duration in minutes.
        /// </summary>
        public int? Duration { get; internal set; }

        /// <summary>
        /// Gets whether a server request of a step action is running.
        /// </summary>
        public bool IsBusy { get; internal set; }

        /// <summary>
        /// Gets whether the last step action failed (the step shows the error with a retry).
        /// </summary>
        public bool HasActionError { get; internal set; }

        /// <summary>
        /// Gets the halls, hosts and host groups of the club; null until the PC selection is first opened.
        /// </summary>
        public BookingHalls? Halls { get; internal set; }

        /// <summary>
        /// Gets the tabs of the PC selection.
        /// </summary>
        public IReadOnlyList<BookingTab> Tabs { get; internal set; } = [];

        /// <summary>
        /// Gets the prices per host group for the chosen session, by host group id.
        /// </summary>
        public IReadOnlyDictionary<int, BookingHostGroupPrice> Prices { get; internal set; } = new Dictionary<int, BookingHostGroupPrice>();

        /// <summary>
        /// Gets the ids of the hosts that are free and have a price for the chosen session.
        /// </summary>
        public IReadOnlySet<int> FreeHostIds { get; internal set; } = new HashSet<int>();

        /// <summary>
        /// Gets the palette position of every host group, by host group id.
        /// </summary>
        public IReadOnlyDictionary<int, int> GroupColors { get; internal set; } = new Dictionary<int, int>();

        /// <summary>
        /// Gets whether availability and prices of the current session are loaded (the PC selection can be shown).
        /// </summary>
        public bool HasAvailability { get; internal set; }

        /// <summary>
        /// Gets the key of the selected tab.
        /// </summary>
        public int SelectedTabKey { get; internal set; }

        /// <summary>
        /// Gets whether the hosts are shown on the map (otherwise as a list).
        /// </summary>
        public bool IsMapView { get; internal set; } = true;

        /// <summary>
        /// Gets the map zoom in percent.
        /// </summary>
        public int Zoom { get; internal set; } = 100;

        /// <summary>
        /// Gets the ids of the selected hosts in the order of selection.
        /// </summary>
        public IReadOnlyList<int> SelectedHostIds { get; internal set; } = [];

        /// <summary>
        /// Gets whether the list of the selected hosts is expanded.
        /// </summary>
        public bool IsSummaryExpanded { get; internal set; } = true;

        /// <summary>
        /// Gets the offer for the selected hosts; null before step 3.
        /// </summary>
        public BookingOffer? Offer { get; internal set; }

        /// <summary>
        /// Gets which amount the user pays.
        /// </summary>
        public BookingAmountKind AmountKind { get; internal set; }

        /// <summary>
        /// Gets the amount that will be paid from the deposit, exactly as sent to the server; null without an offer.
        /// </summary>
        public decimal? PaymentAmount => Offer is null
            ? null
            : AmountKind == BookingAmountKind.Minimum && Offer.MinimumPayment < Offer.Total ? Offer.MinimumPayment : Offer.Total;

        /// <summary>
        /// Gets whether the session (date, start time and duration) is complete.
        /// </summary>
        public bool HasSession => Date is not null && StartTime is not null && Duration is not null;

        /// <summary>
        /// Gets the sum of the prices of the selected hosts (no fee).
        /// </summary>
        public decimal SelectedTotal
        {
            get
            {
                var hosts = Halls?.Hosts;
                if (hosts is null)
                    return 0;

                var total = 0m;
                foreach (var hostId in SelectedHostIds)
                {
                    var host = hosts.FirstOrDefault(h => h.Id == hostId);
                    if (host is not null && Prices.TryGetValue(host.HostGroupId, out var price))
                        total += price.Price ?? 0;
                }

                return total;
            }
        }

        /// <summary>
        /// Gets a host of the club by id; null when unknown.
        /// </summary>
        public BookingHost? GetHost(int hostId) => Halls?.Hosts.FirstOrDefault(host => host.Id == hostId);

        /// <summary>
        /// Gets the name of a host group; empty when unknown.
        /// </summary>
        public string GetGroupName(int groupId) => Halls?.HostGroups.FirstOrDefault(group => group.Id == groupId)?.Name ?? string.Empty;

        /// <summary>
        /// Gets the palette position (0-5) of a host group.
        /// </summary>
        public int GetGroupColor(int groupId) => GroupColors.TryGetValue(groupId, out var index) ? index : 0;

        /// <summary>
        /// Gets the session price of a host of the group; null when the group cannot be booked.
        /// </summary>
        public decimal? GetPrice(int groupId) => Prices.TryGetValue(groupId, out var price) ? price.Price : null;

        /// <summary>
        /// Gets the reference hourly rate of the group; null when the server has none.
        /// </summary>
        public decimal? GetHourlyRate(int groupId) => Prices.TryGetValue(groupId, out var price) ? price.HourlyRate : null;

        internal void Reset()
        {
            Club.Reset();
            Options.Reset();
            Balance.Reset();
            IsNotFound = false;
            Step = 1;
            IsFlowStartedInApp = false;
            Durations = [];
            Date = null;
            StartTime = null;
            Duration = null;
            IsBusy = false;
            HasActionError = false;
            Halls = null;
            ResetSelection();
        }

        /// <summary>
        /// Clears everything that depends on the session: availability, prices, the selected hosts and the offer.
        /// </summary>
        internal void ResetSelection()
        {
            Tabs = [];
            Prices = new Dictionary<int, BookingHostGroupPrice>();
            FreeHostIds = new HashSet<int>();
            HasAvailability = false;
            SelectedTabKey = 0;
            IsMapView = true;
            Zoom = 100;
            SelectedHostIds = [];
            IsSummaryExpanded = true;
            Offer = null;
            AmountKind = BookingAmountKind.Full;
        }
    }
}
