namespace Gizmo.Go.Core.Models.Registration
{
    public enum RegistrationCompleteResultCode
    {
        Success,
        Failure,
        InvalidToken,
        InvalidTokenInput,
        ExpiredToken,
        UsedToken,
        RevokedToken,
        InvalidVerification,
        AlreadyVerified,
        InvalidInput,
        NoUserGroup,
        Unknown
    }
}
