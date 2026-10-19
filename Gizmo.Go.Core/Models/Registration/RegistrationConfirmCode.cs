namespace Gizmo.Go.Core.Models.Registration
{
    public enum RegistrationConfirmCode
    {
        Success,
        Failure,
        InvalidToken,
        ExpiredToken,
        UsedToken,
        InvalidConfirmationCode,
        AlreadyVerified,
        Unknown,
    }
}
