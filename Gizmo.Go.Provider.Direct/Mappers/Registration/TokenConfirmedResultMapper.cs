using Gizmo.Go.Core.Models.Registration;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Registration;

internal static class TokenConfirmedResultMapper
{
    public static TokenConfirmedResult Map(TokenConfirmedResultModel model) =>
        new()
        {
            IsConfirmed = model.IsConfirmed,
            Phone = model.Phone
        };
}
