namespace Gizmo.Go.Core.Models.Registration
{
    public abstract record RegistrationStartResult
    {
        private RegistrationStartResult() { }

        public sealed record CodeInputRequired(string Token, string? Destination, int CodeLength, int ExpiresInSeconds, Guid CapabilityGuid) : RegistrationStartResult;
        public sealed record RedirectRequired(string Token, string RedirectUrl, int ExpiresInSeconds, Guid CapabilityGuid) : RegistrationStartResult;
        public sealed record CallRequired(string Token, string PhoneNumber, int ExpiresInSeconds, Guid CapabilityGuid) : RegistrationStartResult;
        public sealed record Failed(RegistrationStartCode Code) : RegistrationStartResult;
    }
}
