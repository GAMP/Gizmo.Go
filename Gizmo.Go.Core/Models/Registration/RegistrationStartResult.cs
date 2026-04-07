namespace Gizmo.Go.Core.Models.Registration
{
    public sealed class RegistrationStartResult
    {
        public RegistrationStartResultCode Result { get; init; }
        public string? Token { get; init; }
        public string? RedirectUrl { get; init; }
        public int CodeLength { get; init; }
        public int ExpiresInSeconds { get; init; }
    }
}
