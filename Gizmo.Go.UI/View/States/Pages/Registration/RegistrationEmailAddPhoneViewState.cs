using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register()]
    public sealed class RegistrationEmailAddPhoneViewState : ValidatingViewStateBase
    {
        public string Phone { get; internal set; } = string.Empty;

        public string PhoneInput { get; internal set; } = string.Empty;

        public string PhoneE164 { get; internal set; } = string.Empty;

        public string SelectedCountryIso2 { get; internal set; } = string.Empty;

        public string SelectedCountryName { get; internal set; } = string.Empty;

        public string SelectedDialCode { get; internal set; } = string.Empty;

        public string SelectedCountryPlaceholder { get; internal set; } = string.Empty;

        public int PhoneLength { get; internal set; } = 15;

        public bool IsSubmitting { get; internal set; }

        public string? ErrorMessage { get; internal set; }

        public bool CanSubmit => !string.IsNullOrEmpty(PhoneE164) && !IsSubmitting;

        public string ConfirmedEmail { get; internal set; } = string.Empty;
    }
}
