namespace Gizmo.Go.Core.Models.Events
{
    /// <summary>
    /// Event of the current user delivered by the server in real time.
    /// </summary>
    public abstract class UserEvent
    {
    }

    /// <summary>
    /// The balance of the user changed (a signal, also sent with every usage charge): the balance is read separately.
    /// </summary>
    public sealed class UserBalanceChangedEvent : UserEvent
    {
    }

    /// <summary>
    /// A reservation of the user (owner or member) was created or canceled.
    /// </summary>
    public sealed class UserReservationEvent : UserEvent
    {
        public int ReservationId { get; init; }

        public UserReservationEventKind Kind { get; init; }
    }

    public enum UserReservationEventKind
    {
        Created,
        Canceled
    }

    /// <summary>
    /// News or their branches were changed: the visible news list may have changed.
    /// </summary>
    public sealed class NewsChangedEvent : UserEvent
    {
    }
}
