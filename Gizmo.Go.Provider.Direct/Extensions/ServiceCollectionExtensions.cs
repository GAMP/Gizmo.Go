using Gizmo.Go.Core.Configuration;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Services;
using Gizmo.Web.Api.Clients.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Gizmo.Go.Provider.Direct.Extensions
{
    /// <summary>
    /// Direct provider service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Gizmo Go direct provider services.
        /// </summary>
        public static IServiceCollection AddGizmoGoDirect(this IServiceCollection services)
        {
            static void httpClientConfig(IServiceProvider sp, HttpClient client)
            {
                var settings = sp.GetRequiredService<IOptions<GizmoGoSettings>>().Value;
                var baseUrl = settings.BackendUrl;

                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    var navManager = sp.GetRequiredService<NavigationManager>();
                    baseUrl = navManager.BaseUri;
                }

                client.BaseAddress = new Uri(baseUrl);
            }

            // unsecure api clients (auth endpoints - no authorization header needed)
            services.AddUnsecureWebApiClients("GizmoGoDirectUnsecure", httpClientConfig)
                .WithMessagePackSerialization()
                .WithCurrentUICultureMessageHandler();

            // secure api clients (registration, options, etc.)
            services.AddSecureWebApiClients("GizmoGoDirectSecure", httpClientConfig)
                .WithMessagePackSerialization()
                .WithCurrentUICultureMessageHandler();

            services.AddSingleton<IAuthService, DirectAuthService>();
            services.AddSingleton<IBranchProvider, DirectBranchProvider>();
            services.AddSingleton<IRegistrationService, DirectRegistrationService>();

            return services;
        }
    }
}
