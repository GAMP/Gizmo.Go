using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Provider.Direct.Helpers;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Registration
{
    internal static class RegistrationAgreementMapper
    {
        internal static RegistrationAgreement Map(PublicUserAgreementModel source) =>
            new()
            {
                Id           = source.Id,
                Name         = source.Name,
                Agreement    = source.Agreement,
                IsRejectable = source.IsRejectable,
            };
    }
}
