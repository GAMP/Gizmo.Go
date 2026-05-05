using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components
{
    public partial class InfoBlock : ComponentBase
    {
        #region PROPERTIES

        [Parameter]
        public string? Message { get; set; }

        [Parameter]
        public bool ShowIcon { get; set; }

        #endregion
    }
}
