namespace Gizmo.Go.Core.Models.PasswordRecovery
{
    public enum PasswordRecoveryCompleteCode
    {
        Success,
        Failure,
        InvalidToken,
        InvalidTokenInput,
        ExpiredToken,
        UsedToken,
        RevokedToken,
        InvalidTokenType,
        Unknown,
    }
}
