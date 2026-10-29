using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components
{
    /// <summary>
    /// Logo above the auth card title (login, registration providers).
    /// Shows the configured logo image or the empty placeholder square when none is set.
    /// </summary>
    public partial class AuthCardLogo : ComponentBase
    {
        /// <summary>
        /// Logo image URL of the server; empty shows the placeholder.
        /// </summary>
        [Parameter] public string? LogoUrl { get; set; }
    }
}
