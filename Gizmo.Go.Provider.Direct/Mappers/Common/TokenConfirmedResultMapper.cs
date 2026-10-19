using Gizmo.Go.Core.Models.Registration;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Common
{
    internal static class TokenConfirmedResultMapper
    {
        internal static TokenConfirmedResult Map(TokenConfirmedResultModel source) =>
            new()
            {
                IsConfirmed      = source.IsConfirmed,
                Phone            = source.Phone,
                UserAlreadyExists = source.UserAlreadyExists,
            };
    }
}
