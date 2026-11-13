using Gizmo.Go.Core.Models.Settings;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Platform-agnostic persistence of the settings that belong to the device (language, notification switches).
    /// </summary>
    public interface IDeviceSettingsService
    {
        /// <summary>
        /// Gets the stored settings; defaults when nothing is stored or the storage cannot be read.
        /// </summary>
        ValueTask<DeviceSettings> GetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stores the settings; a storage failure is logged and does not throw.
        /// </summary>
        ValueTask SetAsync(DeviceSettings settings, CancellationToken cancellationToken = default);
    }
}
