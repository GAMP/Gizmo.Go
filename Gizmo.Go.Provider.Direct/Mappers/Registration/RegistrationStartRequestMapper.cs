using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Provider.Direct.Helpers;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Registration
{
    internal static class RegistrationStartRequestMapper
    {
        public static UserRegistrationMethodStartModel Map(RegistrationStartRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            return new UserRegistrationMethodStartModel
            {
                MethodId = request.MethodId,
                Value = request.Kind switch
                {
                    RegistrationStartKind.MobilePhone or RegistrationStartKind.Email => request.Value,
                    RegistrationStartKind.Redirect                                   => null,
                    _ => throw new ArgumentOutOfRangeException(nameof(request.Kind), request.Kind, "Unsupported registration start kind.")
                }
            };
        }
    }
}
