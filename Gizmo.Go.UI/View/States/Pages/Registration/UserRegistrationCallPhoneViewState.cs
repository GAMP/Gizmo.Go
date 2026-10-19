using Gizmo.Go.Core.Models.PhoneValidation;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register()]
    public sealed class UserRegistrationCallPhoneViewState : ViewStateBase
    {
        public IReadOnlyList<PhoneCountry> Countries { get; internal set; } = Array.Empty<PhoneCountry>();

        public string RegionCode { get; internal set; } = string.Empty;

        public string PhoneInput { get; internal set; } = string.Empty;

        public string SelectedDialCode { get; internal set; } = string.Empty;

        public string PhoneE164 { get; internal set; } = string.Empty;
    }
}
