using Gizmo.Go.Core.Configuration;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Platform.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Gizmo.Go.Provider.Platform.Extensions
{
    /// <summary>
    /// Platform provider service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Gizmo Go platform provider services.
        /// </summary>
        public static IServiceCollection AddGizmoGoPlatform(this IServiceCollection services)
        {
            services.AddHttpClient("GizmoGoPlatform", (sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<GizmoGoSettings>>().Value;

                if (!string.IsNullOrWhiteSpace(settings.BackendUrl))
                    client.BaseAddress = new Uri(settings.BackendUrl);
            });

            services.AddSingleton<IAuthService, PlatformAuthService>();
            services.AddSingleton<IBranchProvider, PlatformBranchProvider>();

            return services;
        }
    }
}
