using Gizmo.Go.Core.Models.Notification;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Notification
{
    public partial class ToastNotification : ComponentBase
    {
        #region PROPERTIES

        [Parameter]
        public string Message { get; set; } = string.Empty;

        [Parameter]
        public GoNotificationType Type { get; set; } = GoNotificationType.Info;

        [Parameter]
        public INotificationController? Controller { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object>? ExtraParameters { get; set; }

        #endregion

        #region OVERRIDES

        protected string TypeClass => Type switch
        {
            GoNotificationType.Success => "giz-toast--success",
            GoNotificationType.Warning => "giz-toast--warning",
            GoNotificationType.Error   => "giz-toast--error",
            _                          => "giz-toast--info",
        };

        protected void OnDismiss() => Controller?.Dismiss();

        #endregion
    }
}
