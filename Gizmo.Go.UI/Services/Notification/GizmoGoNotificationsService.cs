using Gizmo.Go.Core.Models.Notification;
using Gizmo.Go.Core.Services.Notification;
using Gizmo.Go.UI.Components.Notification;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Go.UI.Services.Notification
{
    internal sealed class GizmoGoNotificationsService : NotificationsServiceBase, IGizmoGoNotificationsService
    {
        public GizmoGoNotificationsService(
            IOptionsMonitor<NotificationsOptions> options,
            IServiceProvider serviceProvider,
            ILogger<GizmoGoNotificationsService> logger)
            : base(options, serviceProvider, logger)
        {
        }

        public Task ShowAsync(string message, GoNotificationType type = GoNotificationType.Info, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { nameof(ToastNotification.Message), message },
                { nameof(ToastNotification.Type), type }
            };

            // ShowNotificationAsync completes synchronously (Task.FromResult); controller is available immediately.
            var task = ShowNotificationAsync<ToastNotification>(parameters, cancellationToken: ct);
            if (task.IsCompleted && task.Result.Controller is { } controller)
                parameters.TryAdd("Controller", controller);

            return Task.CompletedTask;
        }
    }
}
