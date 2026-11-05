using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Home
{
    /// <summary>
    /// Top bar of a main zone screen: a round back button and the screen title.
    /// </summary>
    public partial class ScreenBackHeader : ComponentBase
    {
        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public string AriaLabel { get; set; } = string.Empty;

        [Parameter]
        public EventCallback OnBack { get; set; }

        /// <summary>
        /// Gets or sets whether the title is centered (profile) instead of following the button (news).
        /// </summary>
        [Parameter]
        public bool IsCentered { get; set; }

        /// <summary>
        /// Gets or sets whether the bar lies over an image (news details): no title, a darker button.
        /// </summary>
        [Parameter]
        public bool IsOverlay { get; set; }
    }
}
