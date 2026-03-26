using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class CreateAccountViewState : ViewStateBase
    {
        public string Phone { get; internal set; } = string.Empty;

        public bool TermsAccepted { get; internal set; }

        public bool IsFormValid => TermsAccepted && Phone.Length == 10 && Phone.All(char.IsDigit);
    }
}
