using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Layout
{
    public partial class LoginLayout
    {
        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;
    }
}
