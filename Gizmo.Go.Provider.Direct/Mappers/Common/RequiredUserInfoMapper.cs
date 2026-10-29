using Gizmo.Go.Core.Models;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Common
{
    internal static class RequiredUserInfoMapper
    {
        internal static RequiredUserInfo Map(UserModelRequiredInfo source) =>
            new()
            {
                FirstName = source.FirstName,
                LastName  = source.LastName,
                BirthDate = source.BirthDate,
                Address   = source.Address,
                City      = source.City,
                PostCode  = source.PostCode,
                State     = source.State,
                Country   = source.Country,
                Email     = source.Email,
                Phone     = source.Phone,
                Mobile    = source.Mobile,
                Sex       = source.Sex,
                Password  = source.Password,
            };
    }
}
