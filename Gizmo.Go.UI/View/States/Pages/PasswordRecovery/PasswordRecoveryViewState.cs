using System.Collections.Generic;
using System.Linq;
using System;
using Gizmo.Go.Core.Models.PasswordRecovery;
using Gizmo.Go.UI.View.States;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.PasswordRecovery
{
    [Register]
    public sealed class PasswordRecoveryViewState : ViewStateBase
    {
        public IReadOnlyList<PasswordRecoveryProvider> Providers { get; internal set; } = Array.Empty<PasswordRecoveryProvider>();

        public IReadOnlyList<PasswordRecoveryProvider> PriorityProviders =>
            Providers.Where(p => p.IsPrimary).ToList();

        public IReadOnlyList<PasswordRecoveryProvider> AltProviders =>
            Providers.Where(p => !p.IsPrimary).ToList();

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        public bool ShowAllProviders { get; internal set; }
    }
}
