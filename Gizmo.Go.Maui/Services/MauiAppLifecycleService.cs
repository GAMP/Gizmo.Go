using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Maui.Services
{
    public sealed class MauiAppLifecycleService : IAppLifecycleService
    {
        public event EventHandler? Resumed;

        public Task InitializeAsync() => Task.CompletedTask;

        public Task StartWatchingAsync() => Task.CompletedTask;

        internal void RaiseResumed() => Resumed?.Invoke(this, EventArgs.Empty);
    }
}
