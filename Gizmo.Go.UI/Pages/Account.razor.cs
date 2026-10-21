using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Gizmo.UI.Services;

namespace Gizmo.Go.UI.Pages
{
    [Authorize]
    public partial class Account : ComponentBase
    {
        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;
    }
}
