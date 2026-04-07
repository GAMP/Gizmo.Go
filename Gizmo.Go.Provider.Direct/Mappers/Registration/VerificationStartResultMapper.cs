using Gizmo.Go.Core.Models.Registration;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Registration
{
    internal static class VerificationStartResultMapper
    {
        public static RegistrationStartResult Map(VerificationStartResultModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            return new RegistrationStartResult
            {
                Result = MapResultCode(model.Result),
                Token = model.Token,
                RedirectUrl = model.RedirectUrl,
                CodeLength = model.CodeLength,
                ExpiresInSeconds = model.ExpiresInSeconds
            };
        }

        private static RegistrationStartResultCode MapResultCode(VerificationStartResultCode code) =>
            code switch
            {
                VerificationStartResultCode.Success            => RegistrationStartResultCode.Success,
                VerificationStartResultCode.DeliveryFailed     => RegistrationStartResultCode.DeliveryFailed,
                VerificationStartResultCode.NoRouteForDelivery => RegistrationStartResultCode.NoRouteForDelivery,
                VerificationStartResultCode.InvalidInput       => RegistrationStartResultCode.InvalidInput,
                VerificationStartResultCode.NonUniqueInput     => RegistrationStartResultCode.NonUniqueInput,
                VerificationStartResultCode.InvalidUserId      => RegistrationStartResultCode.InvalidUserId,
                _                                              => RegistrationStartResultCode.Failed
            };
    }
}
