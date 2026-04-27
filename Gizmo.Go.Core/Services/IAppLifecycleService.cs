namespace Gizmo.Go.Core.Services
{
    public interface IAppLifecycleService
    {
        Task InitializeAsync();
        Task StartWatchingAsync();
        event EventHandler Resumed;
    }
}
