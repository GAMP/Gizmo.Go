using Gizmo.Go.Core.Models.Registration;

namespace Gizmo.Go.Provider.Direct.Mappers.Registration;

internal static class RegistrationCompleteResultMapper
{
    public static RegistrationCompleteResult Map(AccountCreationByTokenCompleteResultCode resultCode) =>
        new()
        {
            Result = resultCode switch
            {
                AccountCreationByTokenCompleteResultCode.Success => RegistrationCompleteResult.Success,
                AccountCreationByTokenCompleteResultCode.Failure => RegistrationCompleteResult.Failure,
                AccountCreationByTokenCompleteResultCode.InvalidToken => RegistrationCompleteResult.InvalidToken,
                AccountCreationByTokenCompleteResultCode.InvalidTokenInput => RegistrationCompleteResult.InvalidTokenInput,
                AccountCreationByTokenCompleteResultCode.ExpiredToken => RegistrationCompleteResult.ExpiredToken,
                AccountCreationByTokenCompleteResultCode.UsedToken => RegistrationCompleteResult.UsedToken,
                AccountCreationByTokenCompleteResultCode.RevokedToken => RegistrationCompleteResult.RevokedToken,
                AccountCreationByTokenCompleteResultCode.InvalidVerification => RegistrationCompleteResult.InvalidVerification,
                AccountCreationByTokenCompleteResultCode.AlreadyVerified => RegistrationCompleteResult.AlreadyVerified,
                AccountCreationByTokenCompleteResultCode.InvalidInput => RegistrationCompleteResult.InvalidInput,
                AccountCreationByTokenCompleteResultCode.NoUserGroup => RegistrationCompleteResult.NoUserGroup,
                _ => RegistrationCompleteResult.Unknown
            }
        };
}
