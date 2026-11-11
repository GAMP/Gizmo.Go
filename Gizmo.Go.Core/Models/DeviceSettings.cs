namespace Gizmo.Go.Core.Models
{
    /// <summary>
    /// App settings stored on the device only (never sent to the server).
    /// </summary>
    public sealed class DeviceSettings
    {
        /// <summary>
        /// Gets or sets the chosen UI culture name (for example "en", "pt-BR"); null when the user has not chosen a language.
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// Gets or sets whether notifications about the own reservations and balance are shown.
        /// </summary>
        public bool PersonalNotifications { get; set; } = true;

        /// <summary>
        /// Gets or sets whether notifications about published news are shown.
        /// </summary>
        public bool NewsNotifications { get; set; }
    }
}
