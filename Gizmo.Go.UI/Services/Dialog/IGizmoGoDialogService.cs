using Gizmo.Client.UI;
using Gizmo.UI.Services;

namespace Gizmo.Go.UI.Services.Dialog
{
    public interface IGizmoGoDialogService : IDialogService
    {
        Task<AddDialogResult<UserAgreementResult>> ShowUserAgreementDialogAsync(
            UserAgreementDialogParameters userAgreementDialogParameters,
            bool allowContinueWithoutAccept = false,
            CancellationToken cancellationToken = default);
    }
}
