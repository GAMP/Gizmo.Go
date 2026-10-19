using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Gizmo.Go.Core.Models.PasswordRecovery;
using Gizmo.Go.Core.Models.PhoneValidation;
using Gizmo.Go.UI.View.States;
using Gizmo.UI.View.States;
using Gizmo.UI;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.PasswordRecovery
{
    [Register]
    public sealed class PasswordRecoveryDestinationViewState : ValidatingViewStateBase
    {
        public PasswordRecoveryIdentifierKind IdentifierKind { get; internal set; }

        [ValidatingProperty]
        public string MatchValue { get; internal set; } = string.Empty;

        [ValidatingProperty]
        public string? Country { get; internal set; }

        [ValidatingProperty]
        public string? RegionCode { get; internal set; }

        [ValidatingProperty(IsAsync = true)]
        public string? MobilePhone { get; internal set; }

        public string? PhoneE164 { get; internal set; }

        public IReadOnlyList<PhoneCountry> Countries { get; internal set; } = Array.Empty<PhoneCountry>();

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;
    }
}
