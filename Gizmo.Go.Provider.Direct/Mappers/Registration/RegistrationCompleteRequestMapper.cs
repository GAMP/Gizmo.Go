using Gizmo.Go.Core.Models.Registration;
using Gizmo.Shared;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Registration
{
    internal static class RegistrationCompleteRequestMapper
    {
        public static RegistrationCompleteModel Map(RegistrationCompleteRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            return new RegistrationCompleteModel
            {
                Token = request.Token,
                Password = request.Password,
                Profile = MapProfile(request.Profile)
            };
        }

        private static UserProfileModelCreate MapProfile(RegistrationProfile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);

            return new UserProfileModelCreate
            {
                Username = profile.Username,
                Email = profile.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                BirthDate = profile.BirthDate,
                Address = profile.Address,
                City = profile.City,
                Country = profile.Country,
                PostCode = profile.PostCode,
                Phone = profile.Phone,
                MobilePhone = profile.MobilePhone,
                Sex = MapSex(profile.Sex)
            };
        }

        private static Sex MapSex(UserSex sex) =>
            sex switch
            {
                UserSex.Unspecified => Sex.Unspecified,
                UserSex.Male        => Sex.Male,
                UserSex.Female      => Sex.Female,
                _ => throw new ArgumentOutOfRangeException(nameof(sex), sex, null)
            };
    }
}
