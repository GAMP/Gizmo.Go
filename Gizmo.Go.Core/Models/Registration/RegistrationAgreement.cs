namespace Gizmo.Go.Core.Models.Registration
{
    public sealed class RegistrationAgreement
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Agreement { get; init; }
        public bool IsRejectable { get; init; }
    }
}
