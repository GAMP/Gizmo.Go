using Gizmo.Go.Core.Models.Notification;
using Gizmo.Go.UI.Helpers;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Notification
{
    public partial class ToastNotification : ComponentBase
    {
        [Parameter]
        public string Message { get; set; } = string.Empty;

        [Parameter]
        public GoNotificationType Type { get; set; } = GoNotificationType.Info;

        [Parameter]
        public INotificationController? Controller { get; set; }

        /// <summary>
        /// Gets or sets the route a tap on the toast opens; null for a toast that only shows a message.
        /// </summary>
        [Parameter]
        public string? Route { get; set; }

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object>? ExtraParameters { get; set; }

        protected string TypeClass => Type switch
        {
            GoNotificationType.Success => "giz-toast--success",
            GoNotificationType.Warning => "giz-toast--warning",
            GoNotificationType.Error   => "giz-toast--error",
            _                          => "giz-toast--info",
        };

        protected void OnDismiss() => Controller?.Dismiss();

        protected void OnOpen()
        {
            if (Route is null)
                return;

            Controller?.Dismiss();

            // an in-app entry: Back of the opened page returns to the page the toast was tapped on
            NavigationService.NavigateTo(Route, NavigationHelper.InAppNavigation);
        }
    }
}
