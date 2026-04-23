using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register()]
    public sealed class RegistrationConfirmationViewState : ViewStateBase
    {
        public string Token { get; internal set; } = string.Empty;

        public string MaskedPhone { get; internal set; } = string.Empty;

        public string? ErrorMessage { get; internal set; }

        public bool IsSubmitting { get; internal set; }

        public int CodeLength { get; internal set; } = 6;

        public string[] Digits { get; internal set; } = ["", "", "", "", "", ""];

        public int SecondsLeft { get; internal set; } = 60;

        public bool TimerExpired => SecondsLeft <= 0;

        public bool CanSubmit => Digits.All(d => d.Length == 1);

        public string TimerDisplay => $"{SecondsLeft / 60:D2}:{SecondsLeft % 60:D2}";
    }
}
