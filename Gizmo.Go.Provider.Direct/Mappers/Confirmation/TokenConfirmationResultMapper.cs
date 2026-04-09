using Gizmo.Go.Core.Models.Confirmation;

namespace Gizmo.Go.Provider.Direct.Mappers.Confirmation;

internal static class TokenConfirmationResultMapper
{
    public static TokenConfirmationResult Map(VerificationCompleteResultCode resultCode) =>
        new()
        {
            Result = resultCode switch
            {
                VerificationCompleteResultCode.Success                => TokenConfirmationResultCode.Success,
                VerificationCompleteResultCode.Failure                => TokenConfirmationResultCode.Failure,
                VerificationCompleteResultCode.InvalidToken           => TokenConfirmationResultCode.InvalidToken,
                VerificationCompleteResultCode.InvalidTokenInput      => TokenConfirmationResultCode.InvalidTokenInput,
                VerificationCompleteResultCode.ExpiredToken           => TokenConfirmationResultCode.ExpiredToken,
                VerificationCompleteResultCode.UsedToken              => TokenConfirmationResultCode.UsedToken,
                VerificationCompleteResultCode.RevokedToken           => TokenConfirmationResultCode.RevokedToken,
                VerificationCompleteResultCode.InvalidVerification    => TokenConfirmationResultCode.InvalidVerification,
                VerificationCompleteResultCode.AlreadyVerified        => TokenConfirmationResultCode.AlreadyVerified,
                VerificationCompleteResultCode.InvalidConfirmationCode => TokenConfirmationResultCode.InvalidConfirmationCode,
                VerificationCompleteResultCode.PartialSuccess         => TokenConfirmationResultCode.PartialSuccess,
                _                                                     => TokenConfirmationResultCode.Unknown
            }
        };
}
