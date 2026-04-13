using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class RegistrationPasswordViewState : ViewStateBase
    {
        public string Password { get; internal set; } = string.Empty;

        public string Confirm { get; internal set; } = string.Empty;

        public bool ReqLength => Password.Length >= 8;

        public bool ReqUppercase => Password.Any(char.IsUpper);

        public bool ReqDigit => Password.Any(char.IsDigit);

        public bool ReqSpecial => Password.Any(c => !char.IsLetterOrDigit(c));

        public bool AllRequirementsMet => ReqLength && ReqUppercase && ReqDigit && ReqSpecial;

        public bool Mismatch => Confirm.Length > 0 && Password != Confirm;

        public bool CanSubmit => AllRequirementsMet && Password == Confirm && Confirm.Length > 0;

        public bool IsSubmitting { get; internal set; }
    }
}
