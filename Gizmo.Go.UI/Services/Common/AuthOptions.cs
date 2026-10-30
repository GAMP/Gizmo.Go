namespace Gizmo.Go.UI.Services.Common
{
    /// <summary>
    /// Server options of the auth screens (welcome, login, registration, password recovery).
    /// </summary>
    public sealed class AuthOptions
    {
        /// <summary>
        /// Gets whether the mobile registration is enabled and the server has a default user group to register into.
        /// </summary>
        public bool IsRegistrationEnabled { get; init; }

        public bool IsPasswordRecoveryEnabled { get; init; }

        /// <summary>
        /// Gets the logo image URL; null when no logo is configured.
        /// </summary>
        public string? LogoUrl { get; init; }

        /// <summary>
        /// Gets the default phone country (ISO 3166-1 alpha-2).
        /// </summary>
        public string? CountryCode { get; init; }
    }
}
