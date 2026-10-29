using Gizmo.Go.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.Core.Extensions
{
    /// <summary>
    /// Core service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Gizmo Go core services.
        /// </summary>
        public static IServiceCollection AddGizmoGoCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GizmoGoSettings>(configuration.GetSection("GizmoGo"));

            return services;
        }
    }
}
