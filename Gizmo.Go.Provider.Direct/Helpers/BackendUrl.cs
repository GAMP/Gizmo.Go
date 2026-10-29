using Gizmo.Go.Core.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Gizmo.Go.Provider.Direct.Helpers
{
    internal static class BackendUrl
    {
        /// <summary>
        /// Gets the Gizmo Server base URL: the configured backend, otherwise the app origin.
        /// </summary>
        internal static string Resolve(IServiceProvider serviceProvider)
        {
            var baseUrl = serviceProvider.GetRequiredService<IOptions<GizmoGoSettings>>().Value.BackendUrl;

            if (string.IsNullOrWhiteSpace(baseUrl))
                baseUrl = serviceProvider.GetRequiredService<NavigationManager>().BaseUri;

            return baseUrl;
        }
    }
}
