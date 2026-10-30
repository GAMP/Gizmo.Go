using Gizmo.Go.Core.Models.Registration;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register()]
    public sealed class UserRegistrationProvidersViewState : ViewStateBase
    {
        public IReadOnlyList<RegistrationProvider> Providers { get; internal set; } = [];

        public IReadOnlyList<RegistrationProvider> PriorityProviders =>
            Providers.Where(p => p.IsPrimary).ToList();

        public IReadOnlyList<RegistrationProvider> AltProviders =>
            Providers.Where(p => !p.IsPrimary).ToList();

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string? LogoUrl { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        public Guid? FailedChannelGuid { get; internal set; }

        public bool ShowAllProviders { get; internal set; }

        public IReadOnlyList<RegistrationAgreement> Agreements { get; internal set; } = [];
    }
}
