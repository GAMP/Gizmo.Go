using Gizmo.Go.Core.Models.Confirmation;

namespace Gizmo.Go.Provider.Direct.Mappers.Confirmation;

internal static class TokenConfirmationResultMapper
{
    public static TokenConfirmationResult Map(VerificationCompleteResultCode resultCode) =>
        new()
        {
            Result = resultCode switch
            {
                VerificationCompleteResultCode.Success => TokenConfirmationResult.Success,
                VerificationCompleteResultCode.Failure => TokenConfirmationResult.Failure,
                VerificationCompleteResultCode.InvalidToken => TokenConfirmationResult.InvalidToken,
                VerificationCompleteResultCode.InvalidTokenInput => TokenConfirmationResult.InvalidTokenInput,
                VerificationCompleteResultCode.ExpiredToken => TokenConfirmationResult.ExpiredToken,
                VerificationCompleteResultCode.UsedToken => TokenConfirmationResult.UsedToken,
                VerificationCompleteResultCode.RevokedToken => TokenConfirmationResult.RevokedToken,
                VerificationCompleteResultCode.InvalidVerification => TokenConfirmationResult.InvalidVerification,
                VerificationCompleteResultCode.AlreadyVerified => TokenConfirmationResult.AlreadyVerified,
                VerificationCompleteResultCode.InvalidConfirmationCode => TokenConfirmationResult.InvalidConfirmationCode,
                VerificationCompleteResultCode.PartialSuccess => TokenConfirmationResult.PartialSuccess,
                _ => TokenConfirmationResult.Unknown
            }
        };
}
