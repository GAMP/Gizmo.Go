namespace Gizmo.Go.Core.Models.Registration
{
    public enum RegistrationStartResultCode
    {
        Success,
        Failed,
        DeliveryFailed,
        NoRouteForDelivery,
        InvalidInput,
        NonUniqueInput,
        InvalidUserId
    }
}
