using Gizmo.Go.Core.Models.Notification;

namespace Gizmo.Go.Core.Services.Notification
{
    public interface IGizmoGoNotificationsService
    {
        /// <summary>
        /// Shows a toast; with <paramref name="route"/> a tap on the toast opens that route.
        /// </summary>
        Task ShowAsync(string message, GoNotificationType type = GoNotificationType.Info,
                      string? route = null, CancellationToken ct = default);
    }
}
