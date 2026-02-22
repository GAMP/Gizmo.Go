namespace Gizmo.Go.Core.Configuration
{
    /// <summary>
    /// Provider mode.
    /// </summary>
    public enum ProviderMode
    {
        /// <summary>
        /// Direct connection to a Gizmo Server.
        /// </summary>
        Direct,

        /// <summary>
        /// Platform API (self-hosted or global).
        /// </summary>
        Platform
    }
}
