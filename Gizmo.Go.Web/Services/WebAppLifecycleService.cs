using Gizmo.Go.Core.Services;
using Microsoft.JSInterop;

namespace Gizmo.Go.Web.Services
{
    internal sealed class WebAppLifecycleService : IAppLifecycleService, IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<WebAppLifecycleService>? _selfRef;
        private bool _initialized;

        public event EventHandler? Resumed;

        public WebAppLifecycleService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            if (_initialized)
                return;

            _selfRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("window.gizmoGoLifecycle.register", _selfRef);
            _initialized = true;
        }

        public async Task StartWatchingAsync()
        {
            await _jsRuntime.InvokeVoidAsync("window.gizmoGoLifecycle.startWatching");
        }

        [JSInvokable]
        public void OnResumed()
        {
            Resumed?.Invoke(this, EventArgs.Empty);
        }

        public ValueTask DisposeAsync()
        {
            _selfRef?.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
