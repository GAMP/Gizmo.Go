using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.Services.Registration
{
    public sealed class CountdownTimer : IDisposable
    {
        private CancellationTokenSource? _cts;
        public int SecondsLeft { get; private set; }

        public async Task StartAsync(int seconds, Func<int, Task> onTick, ILogger? logger = null)
        {
            Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            SecondsLeft = seconds;
            await onTick(SecondsLeft);

            try
            {
                while (SecondsLeft > 0 && !token.IsCancellationRequested)
                {
                    await Task.Delay(1000, token);
                    if (token.IsCancellationRequested) break;
                    SecondsLeft--;
                    await onTick(SecondsLeft);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Countdown timer faulted.");
            }
        }

        public void Cancel()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public void Dispose() => Cancel();
    }
}
