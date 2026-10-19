using Gizmo.Client.UI;
using Gizmo.Go.UI.Components.Dialog;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Go.UI.Services.Dialog
{
    public sealed class GizmoGoDialogService : DialogServiceBase, IGizmoGoDialogService
    {
        public GizmoGoDialogService(
            IOptionsMonitor<DialogOptions> options,
            IServiceProvider serviceProvider,
            ILogger<GizmoGoDialogService> logger) : base(options, serviceProvider, logger)
        {
        }

        public Task<AddDialogResult<UserAgreementResult>> ShowUserAgreementDialogAsync(UserAgreementDialogParameters userAgreementDialogParameters, bool allowContinueWithoutAccept = false, CancellationToken cancellationToken = default)
        {
            var parameters = userAgreementDialogParameters.ToDictionary();
            parameters["AllowContinueWithoutAccept"] = allowContinueWithoutAccept;

            return ShowDialogAsync<UserAgreementDialog, UserAgreementResult>(parameters, new DialogDisplayOptions()
            {
                Closable = false,
                CloseOnClick = false
            }, default, cancellationToken);
        }
    }
}
