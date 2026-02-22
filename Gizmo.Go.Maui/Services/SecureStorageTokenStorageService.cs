using System.Text.Json;
using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Services;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.Maui.Services
{
    public sealed class SecureStorageTokenStorageService : ITokenStorageService
    {
        private const string STORAGE_KEY = "gizmo_go_tokens";
        private readonly ILogger<SecureStorageTokenStorageService> _logger;

        public SecureStorageTokenStorageService(ILogger<SecureStorageTokenStorageService> logger)
        {
            _logger = logger;
        }

        public async ValueTask<AuthToken?> GetAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var json = await SecureStorage.Default.GetAsync(STORAGE_KEY);
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                return JsonSerializer.Deserialize<AuthToken>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read tokens from SecureStorage.");
                return null;
            }
        }

        public async ValueTask SetAsync(AuthToken data, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                await SecureStorage.Default.SetAsync(STORAGE_KEY, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write tokens to SecureStorage.");
            }
        }

        public ValueTask ClearAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                SecureStorage.Default.Remove(STORAGE_KEY);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear tokens from SecureStorage.");
            }

            return ValueTask.CompletedTask;
        }
    }
}
