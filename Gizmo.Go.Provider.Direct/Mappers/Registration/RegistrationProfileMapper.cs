using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Provider.Direct.Helpers;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Registration
{
    internal static class RegistrationProfileMapper
    {
        internal static UserProfileModelCreate Map(RegistrationProfile source) =>
            new()
            {
                Username    = source.Username ?? string.Empty,
                FirstName   = source.FirstName,
                LastName    = source.LastName,
                BirthDate   = source.BirthDate,
                Sex         = (Gizmo.Web.Api.Models.Sex)source.Sex,
                Email       = source.Email,
                MobilePhone = source.MobilePhone,
                Phone       = source.Phone,
                Country     = source.Country,
                Address     = source.Address,
                City        = source.City,
                PostCode    = source.PostCode,
            };
    }
}
