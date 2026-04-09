namespace Gizmo.Go.Core.Models.Confirmation;

public enum TokenConfirmationResultCode
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
    PartialSuccess,
    InvalidConfirmationCode,
    Unknown
}
