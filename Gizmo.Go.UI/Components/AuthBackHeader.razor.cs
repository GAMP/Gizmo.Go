using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components
{
    public partial class AuthBackHeader : ComponentBase
    {
        public enum BackIconShape { Text, Circle }

        [Parameter] public EventCallback OnBack { get; set; }
        [Parameter] public string? BackText { get; set; }
        [Parameter] public string AriaLabel { get; set; } = string.Empty;
        [Parameter] public BackIconShape IconShape { get; set; } = BackIconShape.Text;
        [Parameter] public RenderFragment? Trailing { get; set; }

        private string _iconCls => IconShape == BackIconShape.Circle ? "auth-back-btn--icon" : "";
    }
}
