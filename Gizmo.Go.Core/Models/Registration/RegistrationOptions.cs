namespace Gizmo.Go.Core.Models.Registration
{
    public sealed class RegistrationOptions
    {
        public bool IsRegistrationEnabled { get; init; }
        public bool IsDirectRegistrationEnabled { get; init; }
        public bool IsPasswordRecoveryEnabled { get; init; }
    }
}
