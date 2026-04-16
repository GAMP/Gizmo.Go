using Gizmo.Go.Core.Services;
using Gizmo.Go.UI.Services.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGizmoGoUI(this IServiceCollection services)
        {
            services.AddSingleton<IRegistrationSessionService, RegistrationSessionService>();
            services.AddSingleton<IPhoneValidationService, PhoneValidationService>();

            return services;
        }
    }
}
