using Gizmo.Go.Core.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Gizmo.Go.Provider.Direct.Helpers
{
    internal static class BackendUrl
    {
        /// <summary>
        /// Gets the Gizmo Server base URL: the configured backend, otherwise the origin the app was loaded from.
        /// </summary>
        /// <remarks>
        /// The server hosts the app under a sub path (<c>/apps/mobile/</c>), its API and files live at the root.
        /// </remarks>
        internal static string Resolve(IServiceProvider serviceProvider)
        {
            var baseUrl = serviceProvider.GetRequiredService<IOptions<GizmoGoSettings>>().Value.BackendUrl;

            if (!string.IsNullOrWhiteSpace(baseUrl))
                return baseUrl;

            var baseUri = new Uri(serviceProvider.GetRequiredService<NavigationManager>().BaseUri);

            return baseUri.GetLeftPart(UriPartial.Authority) + "/";
        }
    }
}
