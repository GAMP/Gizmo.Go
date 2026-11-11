using System.Text.Json;
using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Services;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.Maui.Services
{
    /// <summary>
    /// Device settings backed by MAUI Preferences (one JSON value; no secrets, so not the secure storage).
    /// </summary>
    public sealed class PreferencesDeviceSettingsService : IDeviceSettingsService
    {
        private const string STORAGE_KEY = "gizmo_go_settings";

        private readonly ILogger<PreferencesDeviceSettingsService> _logger;

        public PreferencesDeviceSettingsService(ILogger<PreferencesDeviceSettingsService> logger)
        {
            _logger = logger;
        }

        public ValueTask<DeviceSettings> GetAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var json = Preferences.Default.Get<string?>(STORAGE_KEY, null);

                if (string.IsNullOrWhiteSpace(json))
                    return ValueTask.FromResult(new DeviceSettings());

                return ValueTask.FromResult(JsonSerializer.Deserialize<DeviceSettings>(json) ?? new DeviceSettings());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read device settings from Preferences.");
                return ValueTask.FromResult(new DeviceSettings());
            }
        }

        public ValueTask SetAsync(DeviceSettings settings, CancellationToken cancellationToken = default)
        {
            try
            {
                Preferences.Default.Set(STORAGE_KEY, JsonSerializer.Serialize(settings));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write device settings to Preferences.");
            }

            return ValueTask.CompletedTask;
        }
    }
}
