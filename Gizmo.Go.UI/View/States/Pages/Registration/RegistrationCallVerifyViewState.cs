using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register()]
    public sealed class RegistrationCallVerifyViewState : ViewStateBase
    {
        public string ServerPhoneNumber { get; internal set; } = string.Empty;

        public int SecondsLeft { get; internal set; } = 120;

        public bool TimerExpired => SecondsLeft <= 0;

        public string TimerDisplay => $"{SecondsLeft / 60:D2}:{SecondsLeft % 60:D2}";

        public double TimerProgress => SecondsLeft <= 0 ? 0.0 : SecondsLeft / 120.0;
    }
}
