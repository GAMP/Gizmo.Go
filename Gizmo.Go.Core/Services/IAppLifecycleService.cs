namespace Gizmo.Go.Core.Services
{
    public interface IAppLifecycleService
    {
        Task InitializeAsync();
        Task StartWatchingAsync();
        Task<bool> IsActiveAsync();
        event EventHandler Resumed;
    }
}
