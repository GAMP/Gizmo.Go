using System.ComponentModel.DataAnnotations;
using Gizmo.UI.View.States;
using Gizmo.Go.Core.Models.PhoneValidation;
using Gizmo.UI;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register()]
    public sealed class UserRegistrationPhoneViewState : ValidatingViewStateBase
    {
        [ValidatingProperty()]
        public string? Country { get; internal set; }

        [ValidatingProperty()]
        public string? RegionCode { get; internal set; }

        [ValidatingProperty(IsAsync = true)]
        public string? MobilePhone { get; internal set; }

        public IReadOnlyList<PhoneCountry> Countries { get; internal set; } = Array.Empty<PhoneCountry>();

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;
    }
}
