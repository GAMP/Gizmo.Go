using Gizmo.Go.Core.Models.Ladder;
using Gizmo.Go.Core.Models.Profile;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class ProfileViewState : ViewStateBase
    {
        public ViewBlock<UserProfileDetails> Profile { get; } = new();

        public ViewBlock<LadderLevel> Level { get; } = new();

        /// <summary>
        /// Gets the avatar initials: first letters of the first and last name, otherwise the first two letters of the username.
        /// </summary>
        public string Initials { get; internal set; } = string.Empty;

        public string Nickname { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets the masked mobile phone; null when there is no phone or the countries needed to mask it could not be loaded.
        /// </summary>
        public string? MaskedPhone { get; internal set; }

        /// <summary>
        /// Gets the "In Gizmo since" line; null when the registration date is unknown.
        /// </summary>
        public string? SinceText { get; internal set; }

        /// <summary>
        /// Gets the name of the current UI language.
        /// </summary>
        public string LanguageName { get; internal set; } = string.Empty;
    }
}
