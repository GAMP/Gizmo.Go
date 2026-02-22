using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.Web.Pages
{
    [Authorize]
    public partial class Home : ComponentBase
    {
    }
}
