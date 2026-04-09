namespace Gizmo.Go.Core.Models.Registration;

public sealed class RegistrationCompleteRequest
{
    public required string Token { get; set; }

    public string? Password { get; set; }

    public required RegistrationProfile Profile { get; set; }
}
