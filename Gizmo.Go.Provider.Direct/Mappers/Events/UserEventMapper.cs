using Gizmo.Go.Core.Models.Events;
using Gizmo.Web.Api.Messaging;

namespace Gizmo.Go.Provider.Direct.Mappers.Events
{
    internal static class UserEventMapper
    {
        /// <summary>
        /// Maps a server user event message; null for a message the app does not use.
        /// </summary>
        internal static UserEvent? Map(IUserEventMessage source) =>
            source switch
            {
                UserBalanceChangedUserEventMessage => new UserBalanceChangedEvent(),
                UserReservationCreatedUserEventMessage created => new UserReservationEvent { ReservationId = created.ReservationId, Kind = UserReservationEventKind.Created },
                UserReservationCancelledUserEventMessage cancelled => new UserReservationEvent { ReservationId = cancelled.ReservationId, Kind = UserReservationEventKind.Canceled },
                NewsChangedUserEventMessage => new NewsChangedEvent(),
                _ => null
            };
    }
}
