using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register()]
    public sealed class RegistrationCallPhoneViewState : ValidatingViewStateBase
    {
        [ValidatingProperty()]
        public string PhoneInput { get; internal set; } = string.Empty;

        public string SelectedCountryIso2 { get; internal set; } = string.Empty;

        public string SelectedCountryFlag { get; internal set; } = string.Empty;

        public string SelectedDialCode { get; internal set; } = string.Empty;

        public string SelectedCountryPlaceholder { get; internal set; } = string.Empty;

        public string FormattedPhoneInput { get; internal set; } = string.Empty;

        public string PhoneE164 { get; internal set; } = string.Empty;

        public int PhoneLength { get; internal set; } = 15;

        public Guid IntegrationPublicId { get; internal set; }
    }
}
