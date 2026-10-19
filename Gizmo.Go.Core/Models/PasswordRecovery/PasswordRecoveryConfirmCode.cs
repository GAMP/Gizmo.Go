namespace Gizmo.Go.Core.Models.PasswordRecovery
{
    public enum PasswordRecoveryConfirmCode
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
