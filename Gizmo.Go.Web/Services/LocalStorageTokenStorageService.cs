using System.Text.Json;
using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Gizmo.Go.Web.Services
{
    /// <summary>
    /// Token storage backed by browser localStorage.
    /// </summary>
    public sealed class LocalStorageTokenStorageService : ITokenStorageService
    {
        private const string STORAGE_KEY = "gizmo_go_tokens";

        private readonly IJSRuntime _jsRuntime;
        private readonly ILogger<LocalStorageTokenStorageService> _logger;

        public LocalStorageTokenStorageService(IJSRuntime jsRuntime, ILogger<LocalStorageTokenStorageService> logger)
        {
            _jsRuntime = jsRuntime;
            _logger = logger;
        }

        public async ValueTask<AuthToken?> GetAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", cancellationToken, STORAGE_KEY);

                if (string.IsNullOrWhiteSpace(json))
                    return null;

                return JsonSerializer.Deserialize<AuthToken>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read tokens from localStorage.");
                return null;
            }
        }

        public async ValueTask SetAsync(AuthToken data, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", cancellationToken, STORAGE_KEY, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write tokens to localStorage.");
            }
        }

        public async ValueTask ClearAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", cancellationToken, STORAGE_KEY);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear tokens from localStorage.");
            }
        }
    }
}
