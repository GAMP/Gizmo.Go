using Gizmo.Client.Options;

namespace Gizmo.Go.Core.Models
{
    /// <summary>
    /// Server options available before user login.
    /// </summary>
    public sealed class MobileOptions
    {
        /// <summary>
        /// Gets whether mobile registration is enabled.
        /// </summary>
        public bool IsRegistrationEnabled { get; init; }

        /// <summary>
        /// Gets whether mobile password recovery is enabled.
        /// </summary>
        public bool IsPasswordRecoveryEnabled { get; init; }

        /// <summary>
        /// Gets the logo image URL for the auth screens; null when no logo is configured.
        /// </summary>
        public string? LogoUrl { get; init; }

        /// <summary>
        /// Gets the start UI culture of the server (used when no culture is stored).
        /// </summary>
        public string? PreferredLanguage { get; init; }

        /// <summary>
        /// Gets the default phone country (ISO 3166-1 alpha-2).
        /// </summary>
        public string? CountryCode { get; init; }

        /// <summary>
        /// Gets the password rules.
        /// </summary>
        public PasswordValidationOptions PasswordValidation { get; init; } = new();
    }
}
