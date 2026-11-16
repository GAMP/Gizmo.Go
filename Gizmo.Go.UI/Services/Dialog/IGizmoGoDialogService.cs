using Gizmo.Client.UI;
using Gizmo.UI.Services;

namespace Gizmo.Go.UI.Services.Dialog
{
    public interface IGizmoGoDialogService : IDialogService
    {
        Task<AddDialogResult<UserAgreementResult>> ShowUserAgreementDialogAsync(
            UserAgreementDialogParameters userAgreementDialogParameters,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Shows an agreement for reading only: no accept checkbox, a single close button.
        /// </summary>
        Task<AddDialogResult<UserAgreementResult>> ShowReadOnlyUserAgreementDialogAsync(
            UserAgreementDialogParameters userAgreementDialogParameters,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Shows a confirmation dialog. The result is <see cref="AddComponentResultCode.Ok"/> when confirmed;
        /// "Cancel", the close by an overlay click and a global cancellation give any other code.
        /// </summary>
        /// <param name="isDanger">Shows the confirm button in the danger color (irreversible actions).</param>
        Task<AddDialogResult<EmptyComponentResult>> ShowConfirmDialogAsync(
            string title,
            string text,
            string confirmText,
            bool isDanger = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Shows the "How Ladder works" dialog.
        /// </summary>
        Task<AddDialogResult<EmptyComponentResult>> ShowLadderInfoDialogAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Shows a rules dialog (club rules, booking rules) with the specified title.
        /// </summary>
        Task<AddDialogResult<EmptyComponentResult>> ShowRulesDialogAsync(
            string title,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Shows the "no options" dialog of the booking. The result is <see cref="AddComponentResultCode.Ok"/> for
        /// "Choose another time"; "Close" and the overlay click give any other code.
        /// </summary>
        Task<AddDialogResult<EmptyComponentResult>> ShowBookingNoOptionsDialogAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Shows the "host already taken" sheet of the booking with the hosts that are no longer free.
        /// </summary>
        /// <param name="hosts">Hosts that are no longer free.</param>
        /// <param name="isFinal">Whether the booking itself was refused: the sheet offers only the way to Home.</param>
        Task<AddDialogResult<BookingHostTakenResult>> ShowBookingHostTakenDialogAsync(
            IReadOnlyList<BookingHostTakenItem> hosts,
            bool isFinal = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Shows the "Booked!" dialog with the summary of the reservation.
        /// </summary>
        /// <param name="hosts">Hosts, e.g. "PC 09, PC 15".</param>
        /// <param name="period">Date and time, e.g. "18 Mar · 09:00 – 12:00".</param>
        /// <param name="cost">Cost of the session, formatted.</param>
        Task<AddDialogResult<BookingSuccessResult>> ShowBookingSuccessDialogAsync(
            string hosts,
            string period,
            string cost,
            CancellationToken cancellationToken = default);
    }
}
