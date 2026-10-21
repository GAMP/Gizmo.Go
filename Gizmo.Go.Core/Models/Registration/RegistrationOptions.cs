namespace Gizmo.Go.Core.Models.Registration
{
    public sealed class RegistrationOptions
    {
        public bool IsRegistrationEnabled { get; init; }
        public bool IsDirectRegistrationEnabled { get; init; }
        public bool IsPasswordRecoveryEnabled { get; init; }

        /// <summary>
        /// Logo image URL for the auth screens; null or empty when not configured.
        /// </summary>
        public string? Logo { get; init; }
    }
}
