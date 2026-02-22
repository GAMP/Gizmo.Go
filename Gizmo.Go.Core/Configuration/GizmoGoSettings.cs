namespace Gizmo.Go.Core.Configuration
{
    /// <summary>
    /// Gizmo Go application settings.
    /// </summary>
    public sealed class GizmoGoSettings
    {
        /// <summary>
        /// Gets or sets backend API URL.
        /// If null, resolved from current origin.
        /// </summary>
        public string? BackendUrl { get; set; }

        /// <summary>
        /// Gets or sets provider mode.
        /// </summary>
        public ProviderMode Provider { get; set; }
    }
}
