using Gizmo.Go.UI.Helpers;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class ProfileLanguageViewState : ViewStateBase
    {
        /// <summary>
        /// Gets the app languages sorted by the native name.
        /// </summary>
        public IReadOnlyList<AppLanguages.AppLanguage> Languages { get; internal set; } = [];

        /// <summary>
        /// Gets the code of the current language; null when the current culture matches none of the app languages.
        /// </summary>
        public string? CurrentCode { get; internal set; }

        /// <summary>
        /// Gets whether a language change is in progress (the app is about to reload).
        /// </summary>
        public bool IsSwitching { get; internal set; }
    }
}
