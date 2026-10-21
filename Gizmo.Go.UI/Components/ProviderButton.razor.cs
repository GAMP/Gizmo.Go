using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Go.UI.Components
{
    /// <summary>
    /// Provider / method choice button shared by the registration and password recovery flows.
    /// </summary>
    public partial class ProviderButton : ComponentBase
    {
        /// <summary>
        /// Priority provider (primary button color); otherwise the glass alternative style.
        /// </summary>
        [Parameter] public bool IsPrimary { get; set; }

        /// <summary>
        /// Communication channel used to pick the icon when no custom <see cref="Icon"/> is set.
        /// </summary>
        [Parameter] public Guid? ChannelGuid { get; set; }

        [Parameter] public RenderFragment? Icon { get; set; }

        [Parameter] public string Text { get; set; } = string.Empty;

        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    }
}
