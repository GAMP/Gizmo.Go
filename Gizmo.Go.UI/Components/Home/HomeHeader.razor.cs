using Gizmo.Go.Core.Models.Ladder;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Home
{
    /// <summary>
    /// Home header: logo, ladder level chip (hidden without a level) and the profile button.
    /// </summary>
    public partial class HomeHeader : ComponentBase
    {
        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter]
        public string? LogoUrl { get; set; }

        [Parameter]
        public LadderLevel? Level { get; set; }

        [Parameter]
        public EventCallback OnLevelClick { get; set; }

        [Parameter]
        public EventCallback OnProfileClick { get; set; }
    }
}
