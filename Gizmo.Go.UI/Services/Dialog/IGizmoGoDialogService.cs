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
    }
}
