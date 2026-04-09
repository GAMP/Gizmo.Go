using Gizmo.Go.Core.Models.Registration;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages;

[Register()]
public sealed class RegistrationProvidersViewState : ViewStateBase
{
    public IReadOnlyList<RegistrationProvider> Providers { get; internal set; } = [];
    
    public bool IsLoading { get; internal set; }
}