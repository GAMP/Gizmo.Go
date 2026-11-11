using Gizmo.Go.Core.Models.Agreements;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Agreements
{
    internal static class UserAgreementMapper
    {
        internal static UserAgreement Map(PublicUserAgreementModel source) =>
            new()
            {
                Id           = source.Id,
                Name         = source.Name,
                Text         = source.Agreement,
                DisplayOrder = source.DisplayOrder,
            };
    }
}
