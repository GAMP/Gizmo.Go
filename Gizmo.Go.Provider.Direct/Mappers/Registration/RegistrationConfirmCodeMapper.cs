using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Provider.Direct.Helpers;

namespace Gizmo.Go.Provider.Direct.Mappers.Registration
{
    internal static class RegistrationConfirmCodeMapper
    {
        internal static RegistrationConfirmCode Map(Gizmo.VerificationCompleteResultCode source) =>
            source switch
            {
                Gizmo.VerificationCompleteResultCode.Success                => RegistrationConfirmCode.Success,
                Gizmo.VerificationCompleteResultCode.Failure                => RegistrationConfirmCode.Failure,
                Gizmo.VerificationCompleteResultCode.InvalidToken           => RegistrationConfirmCode.InvalidToken,
                Gizmo.VerificationCompleteResultCode.ExpiredToken           => RegistrationConfirmCode.ExpiredToken,
                Gizmo.VerificationCompleteResultCode.UsedToken              => RegistrationConfirmCode.UsedToken,
                Gizmo.VerificationCompleteResultCode.InvalidConfirmationCode => RegistrationConfirmCode.InvalidConfirmationCode,
                Gizmo.VerificationCompleteResultCode.AlreadyVerified        => RegistrationConfirmCode.AlreadyVerified,
                _                                                            => RegistrationConfirmCode.Unknown,
            };
    }
}
