using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register()]
    public sealed class RegistrationEmailConfirmationViewState : ViewStateBase
    {
        public string Email { get; internal set; } = string.Empty;

        public string? ErrorMessage { get; internal set; }

        public bool IsSubmitting { get; internal set; }

        public string[] Digits { get; internal set; } = new string[] { "", "", "", "", "", "" };

        public int SecondsLeft { get; internal set; } = 60;

        public bool TimerExpired => SecondsLeft <= 0;

        public bool CanSubmit => Digits.All(d => d.Length == 1);

        public string TimerDisplay => $"{SecondsLeft / 60:D2}:{SecondsLeft % 60:D2}";
    }
}
