using Gizmo.Go.Core.Models.Registration;

namespace Gizmo.Go.Provider.Direct.Mappers.Registration;

internal static class RegistrationCompleteResultMapper
{
    public static RegistrationCompleteResult Map(AccountCreationByTokenCompleteResultCode resultCode) =>
        new()
        {
            Result = resultCode switch
            {
                AccountCreationByTokenCompleteResultCode.Success            => RegistrationCompleteResultCode.Success,
                AccountCreationByTokenCompleteResultCode.Failure            => RegistrationCompleteResultCode.Failure,
                AccountCreationByTokenCompleteResultCode.InvalidToken       => RegistrationCompleteResultCode.InvalidToken,
                AccountCreationByTokenCompleteResultCode.InvalidTokenInput  => RegistrationCompleteResultCode.InvalidTokenInput,
                AccountCreationByTokenCompleteResultCode.ExpiredToken       => RegistrationCompleteResultCode.ExpiredToken,
                AccountCreationByTokenCompleteResultCode.UsedToken          => RegistrationCompleteResultCode.UsedToken,
                AccountCreationByTokenCompleteResultCode.RevokedToken       => RegistrationCompleteResultCode.RevokedToken,
                AccountCreationByTokenCompleteResultCode.InvalidVerification => RegistrationCompleteResultCode.InvalidVerification,
                AccountCreationByTokenCompleteResultCode.AlreadyVerified    => RegistrationCompleteResultCode.AlreadyVerified,
                AccountCreationByTokenCompleteResultCode.InvalidInput       => RegistrationCompleteResultCode.InvalidInput,
                AccountCreationByTokenCompleteResultCode.NoUserGroup        => RegistrationCompleteResultCode.NoUserGroup,
                _                                                           => RegistrationCompleteResultCode.Unknown
            }
        };
}
