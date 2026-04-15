using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components
{
    public partial class ErrorBlock : ComponentBase
    {
        #region PROPERTIES

        [Parameter]
        public string? Message { get; set; }

        #endregion
    }
}
