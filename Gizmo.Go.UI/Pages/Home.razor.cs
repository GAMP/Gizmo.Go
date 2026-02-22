using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages
{
    [Authorize]
    public partial class Home : ComponentBase
    {
    }
}
