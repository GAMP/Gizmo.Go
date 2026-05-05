namespace Gizmo.Go.UI.Services.Registration
{
    public sealed record RegistrationSnapshot(
        string Token = "",
        string Password = "",
        string Phone = "",
        string Email = "",
        int CodeLength = 0,
        RegistrationFlow Flow = RegistrationFlow.None);
}
