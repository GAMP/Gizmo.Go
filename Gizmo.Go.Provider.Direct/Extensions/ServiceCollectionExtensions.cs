using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Handlers;
using Gizmo.Go.Provider.Direct.Helpers;
using Gizmo.Go.Provider.Direct.Services;
using Gizmo.Web.Api.Clients.Builder;
using Microsoft.Extensions.DependencyInjection;

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
                client.BaseAddress = new Uri(BackendUrl.Resolve(sp));
            }

            services.AddTransient<MobileAppHeaderHandler>();
            services.AddTransient<BearerTokenHandler>();

            // unsecure api clients (auth endpoints - no authorization header needed)
            services.AddUnsecureWebApiClients("GizmoGoDirectUnsecure", httpClientConfig)
                .WithMessagePackSerialization()
                .WithCurrentUICultureMessageHandler()
                .WithMessageHandler<MobileAppHeaderHandler>();

            // secure api clients (current user profile, agreements): the user token is attached to every request
            services.AddSecureWebApiClients("GizmoGoDirectSecure", httpClientConfig)
                .WithMessagePackSerialization()
                .WithCurrentUICultureMessageHandler()
                .WithMessageHandler<MobileAppHeaderHandler>()
                .WithMessageHandler<BearerTokenHandler>();
            
            services.AddSingleton<IAuthService, DirectAuthService>();
            services.AddSingleton<IBranchProvider, DirectBranchProvider>();
            services.AddSingleton<IUserRegistrationService, DirectUserRegistrationService>();
            services.AddSingleton<IPasswordRecoveryService, DirectPasswordRecoveryService>();
            services.AddSingleton<IPhoneValidationService, DirectPhoneValidationService>();
            services.AddSingleton<IMobileOptionsService, DirectMobileOptionsService>();
            services.AddSingleton<IUserProfileService, DirectUserProfileService>();
            services.AddSingleton<IPendingUserAgreementsService, DirectPendingUserAgreementsService>();

            return services;
        }
    }
}
