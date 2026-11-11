using Gizmo.Go.Core.Models.Events;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformUserEventsService : IUserEventsService
    {
        public event EventHandler<UserEvent>? EventReceived
        {
            add { }
            remove { }
        }

        public Task StartAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task StopAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
