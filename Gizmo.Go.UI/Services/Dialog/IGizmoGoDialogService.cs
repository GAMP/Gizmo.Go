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
    }
}
