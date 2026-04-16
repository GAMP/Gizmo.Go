using Gizmo.Go.Core.Models.Notification;

namespace Gizmo.Go.Core.Services.Notification
{
    public interface IGizmoGoNotificationsService
    {
        Task ShowAsync(string message, GoNotificationType type = GoNotificationType.Info,
                      CancellationToken ct = default);
    }
}
