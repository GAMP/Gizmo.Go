using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class CreateAccountViewState : ValidatingViewStateBase
    {
        public string PhoneInput { get; internal set; } = string.Empty;

        public string SelectedCountryIso2 { get; internal set; } = string.Empty;

        public string SelectedCountryName { get; internal set; } = string.Empty;

        public string SelectedDialCode { get; internal set; } = string.Empty;

        public string PhoneE164 { get; internal set; } = string.Empty;

        public bool IsPhoneValid { get; internal set; }

        public string? PhoneValidationMessage { get; internal set; }

        public bool TermsAccepted { get; internal set; }

        public string? TermsValidationMessage { get; internal set; }

        public bool IsFormValid =>
            IsPhoneValid &&
            !string.IsNullOrWhiteSpace(PhoneE164) &&
            TermsAccepted;
    }
}
