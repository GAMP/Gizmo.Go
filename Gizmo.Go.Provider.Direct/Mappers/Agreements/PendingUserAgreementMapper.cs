using Gizmo.Go.Core.Models.Agreements;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Agreements
{
    internal static class PendingUserAgreementMapper
    {
        internal static PendingUserAgreement Map(UserAgreementModel source) =>
            new()
            {
                Id           = source.Id,
                Name         = source.Name,
                Agreement    = source.Agreement,
                IsRejectable = source.IsRejectable,
                IgnoreState  = source.IgnoreState,
            };
    }
}
