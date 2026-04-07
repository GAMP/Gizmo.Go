using Gizmo.Go.Core.Models.Registration;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Registration
{
    internal static class RegistrationStartRequestMapper
    {
        public static RegistrationStartModel Map(RegistrationStartRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            return new RegistrationStartModel
            {
                IntegrationPublicId = request.IntegrationPublicId,
                DeliveryMethod = MapDeliveryMethod(request.DeliveryMethod),
                PhoneNumber = request.PhoneNumber,
                Email = request.Email
            };
        }

        private static VerificationDeliveryMethod MapDeliveryMethod(RegistrationDeliveryMethod method) =>
            method switch
            {
                RegistrationDeliveryMethod.Redirect     => VerificationDeliveryMethod.Redirect,
                RegistrationDeliveryMethod.CodeDispatch => VerificationDeliveryMethod.CodeDispatch,
                _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
            };
    }
}
