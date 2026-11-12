using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Home
{
    /// <summary>
    /// Quick actions row: clubs on the map, my reservations, QR login. The map and QR login are inactive in the first version.
    /// </summary>
    public partial class QuickActions : ComponentBase
    {
        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        /// <summary>
        /// Gets or sets the callback raised when "My reservations" is tapped.
        /// </summary>
        [Parameter]
        public EventCallback OnOpenReservations { get; set; }
    }
}
