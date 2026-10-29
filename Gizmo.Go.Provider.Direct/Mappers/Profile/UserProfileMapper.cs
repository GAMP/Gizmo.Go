using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Models.Profile;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Profile
{
    internal static class UserProfileMapper
    {
        internal static UserProfileDetails Map(UserProfileModel source) =>
            new()
            {
                FirstName   = source.FirstName,
                LastName    = source.LastName,
                BirthDate   = source.BirthDate,
                Sex         = (UserSex)source.Sex,
                Email       = source.Email,
                MobilePhone = source.MobilePhone,
                Phone       = source.Phone,
                Address     = source.Address,
                City        = source.City,
                PostCode    = source.PostCode,
                Country     = source.Country,
            };

        internal static UserProfileModelUserUpdate Map(UserProfileUpdate source) =>
            new()
            {
                FirstName   = source.FirstName,
                LastName    = source.LastName,
                BirthDate   = source.BirthDate,
                Sex         = source.Sex is { } sex ? (Sex)sex : null,
                Email       = source.Email,
                MobilePhone = source.MobilePhone,
                Phone       = source.Phone,
                Address     = source.Address,
                City        = source.City,
                PostCode    = source.PostCode,
                Country     = source.Country,
            };
    }
}
