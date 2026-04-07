namespace Gizmo.Go.Core.Models.Registration
{
    public sealed class RegistrationStartRequest
    {
        public Guid IntegrationPublicId { get; init; }
        public RegistrationDeliveryMethod DeliveryMethod { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Email { get; init; }
    }
}
