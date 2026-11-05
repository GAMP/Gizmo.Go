using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Home
{
    /// <summary>
    /// Quick actions row: clubs on the map, booking, QR login. Inactive in the first version.
    /// </summary>
    public partial class QuickActions : ComponentBase
    {
        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;
    }
}
