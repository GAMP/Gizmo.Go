using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Notification
{
    public partial class NotificationsHost : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        private NotificationsHostViewState NotificationsHostViewState { get; set; } = null!;

        [Inject]
        private NotificationsHostViewService NotificationsHostViewService { get; set; } = null!;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(NotificationsHostViewState);
        }

        public void Dispose()
        {
            this.UnsubscribeChange(NotificationsHostViewState);
        }

        #endregion
    }
}
