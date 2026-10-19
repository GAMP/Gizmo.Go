using Gizmo.Go.Core.Services;
using Gizmo.Go.Core.Services.Notification;
using Gizmo.Go.UI.Services.Dialog;
using Gizmo.Go.UI.Services.Notification;
using Gizmo.Go.UI.Services.PasswordRecovery;
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
            services.AddSingleton<IPasswordRecoverySessionService, PasswordRecoverySessionService>();

            services.AddSingleton<GizmoGoNotificationsService>();
            services.AddSingleton<IGizmoGoNotificationsService>(sp => sp.GetRequiredService<GizmoGoNotificationsService>());
            services.AddSingleton<INotificationsService>(sp => sp.GetRequiredService<GizmoGoNotificationsService>());
            services.AddSingleton<NotificationsHostViewState>();
            services.AddSingleton<NotificationsHostViewService>();

            services.AddSingleton<GizmoGoDialogService>();
            services.AddSingleton<IGizmoGoDialogService>(sp => sp.GetRequiredService<GizmoGoDialogService>());
            services.AddSingleton<IDialogService>(sp => sp.GetRequiredService<GizmoGoDialogService>());
            services.AddSingleton<DialogHostViewState>();
            services.AddSingleton<DialogHostViewStateService>();
            // constructed on startup so it subscribes to DialogChanged before the first dialog is shown
            services.AddSingleton<IViewService>(sp => sp.GetRequiredService<DialogHostViewStateService>());

            return services;
        }
    }
}
