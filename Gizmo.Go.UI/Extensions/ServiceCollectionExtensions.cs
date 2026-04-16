using Gizmo.Go.Core.Services;
using Gizmo.Go.Core.Services.Notification;
using Gizmo.Go.Core.Services.Realtime;
using Gizmo.Go.UI.Services.Notification;
using Gizmo.Go.UI.Services.Realtime;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGizmoGoUI(this IServiceCollection services)
        {
            services.AddSingleton<IRegistrationSessionService, RegistrationSessionService>();
            services.AddSingleton<IPhoneValidationService, PhoneValidationService>();

            services.AddSingleton<GizmoGoNotificationsService>();
            services.AddSingleton<IGizmoGoNotificationsService>(sp => sp.GetRequiredService<GizmoGoNotificationsService>());
            services.AddSingleton<INotificationsService>(sp => sp.GetRequiredService<GizmoGoNotificationsService>());
            services.AddSingleton<NotificationsHostViewState>();
            services.AddSingleton<NotificationsHostViewService>();

            services.AddSingleton<IRealtimeEventService, SignalRRealtimeEventService>();

            return services;
        }
    }
}
