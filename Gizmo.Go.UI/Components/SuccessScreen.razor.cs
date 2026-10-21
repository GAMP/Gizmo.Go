using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components
{
    /// <summary>
    /// Success step shared by registration and password recovery: check icon, title, subtitle,
    /// an optional block (e.g. the contact card) and the primary button.
    /// Figma 159:13238 (registration), 159:12653 (password recovery).
    /// </summary>
    public partial class SuccessScreen : ComponentBase
    {
        [Parameter] public string Title { get; set; } = string.Empty;

        [Parameter] public RenderFragment? Subtitle { get; set; }

        /// <summary>
        /// Optional block between the text and the button.
        /// </summary>
        [Parameter] public RenderFragment? ChildContent { get; set; }

        [Parameter] public string ButtonText { get; set; } = string.Empty;

        [Parameter] public EventCallback OnButtonClick { get; set; }

        /// <summary>
        /// Layout modifier for the root element, e.g. "registration-success--recovery".
        /// </summary>
        [Parameter] public string? CssClass { get; set; }
    }
}
