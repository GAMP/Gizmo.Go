namespace Gizmo.Go.Core.Models.PasswordRecovery
{
    public enum PasswordRecoveryStartCode
    {
        Success,
        Failed,
        NoRouteForDelivery,
        DeliveryFailed,
        InvalidInput,
        NonUniqueInput,
        InvalidUserId,
        UserNotFound,
        Unknown
    }
}
