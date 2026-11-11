using System.Text.Json;
using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Gizmo.Go.Web.Services
{
    /// <summary>
    /// Device settings backed by browser localStorage (one JSON value).
    /// </summary>
    public sealed class LocalStorageDeviceSettingsService : IDeviceSettingsService
    {
        private const string STORAGE_KEY = "gizmo_go_settings";

        private readonly IJSRuntime _jsRuntime;
        private readonly ILogger<LocalStorageDeviceSettingsService> _logger;

        public LocalStorageDeviceSettingsService(IJSRuntime jsRuntime, ILogger<LocalStorageDeviceSettingsService> logger)
        {
            _jsRuntime = jsRuntime;
            _logger = logger;
        }

        public async ValueTask<DeviceSettings> GetAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", cancellationToken, STORAGE_KEY);

                if (string.IsNullOrWhiteSpace(json))
                    return new DeviceSettings();

                return JsonSerializer.Deserialize<DeviceSettings>(json) ?? new DeviceSettings();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read device settings from localStorage.");
                return new DeviceSettings();
            }
        }

        public async ValueTask SetAsync(DeviceSettings settings, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", cancellationToken, STORAGE_KEY, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write device settings to localStorage.");
            }
        }
    }
}
