using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Models.Profile;

namespace Gizmo.Go.UI.Services.Profile
{
    /// <summary>
    /// Profile fields shown on the profile completion step.
    /// </summary>
    public sealed class ProfileCompletionFields
    {
        public static readonly ProfileCompletionFields None = new();

        public bool FirstName { get; init; }
        public bool LastName { get; init; }
        public bool BirthDate { get; init; }
        public bool Sex { get; init; }
        public bool Email { get; init; }
        public bool MobilePhone { get; init; }
        public bool Phone { get; init; }
        public bool Address { get; init; }
        public bool City { get; init; }
        public bool PostCode { get; init; }
        public bool Country { get; init; }

        public bool Any => FirstName || LastName || BirthDate || Sex || Email || MobilePhone || Phone || Address || City || PostCode || Country;

        /// <summary>
        /// Requested mode (the server asked for the profile): all fields required by the group.
        /// Incomplete mode: only the required fields the user has no value for.
        /// </summary>
        public static ProfileCompletionFields Create(RequiredUserInfo required, UserProfileDetails profile, bool isPersonalInfoRequested)
        {
            return new ProfileCompletionFields()
            {
                FirstName = IsShown(isPersonalInfoRequested, required.FirstName, string.IsNullOrEmpty(profile.FirstName)),
                LastName = IsShown(isPersonalInfoRequested, required.LastName, string.IsNullOrEmpty(profile.LastName)),
                BirthDate = IsShown(isPersonalInfoRequested, required.BirthDate, profile.BirthDate == null),
                Sex = IsShown(isPersonalInfoRequested, required.Sex, profile.Sex == UserSex.Unspecified),
                Email = IsShown(isPersonalInfoRequested, required.Email, string.IsNullOrEmpty(profile.Email)),
                MobilePhone = IsShown(isPersonalInfoRequested, required.Mobile, IsPhoneMissing(profile.MobilePhone)),
                Phone = IsShown(isPersonalInfoRequested, required.Phone, IsPhoneMissing(profile.Phone)),
                Address = IsShown(isPersonalInfoRequested, required.Address, string.IsNullOrEmpty(profile.Address)),
                City = IsShown(isPersonalInfoRequested, required.City, string.IsNullOrEmpty(profile.City)),
                PostCode = IsShown(isPersonalInfoRequested, required.PostCode, string.IsNullOrEmpty(profile.PostCode)),
                Country = IsShown(isPersonalInfoRequested, required.Country, string.IsNullOrEmpty(profile.Country)),
            };
        }

        /// <summary>
        /// A phone without digits is missing: a cleared phone can be stored as a bare "+" (or a mask without digits).
        /// </summary>
        public static bool IsPhoneMissing(string? phone)
        {
            return string.IsNullOrEmpty(phone) || !phone.Any(char.IsDigit);
        }

        private static bool IsShown(bool isPersonalInfoRequested, bool isRequired, bool isMissing)
        {
            return isRequired && (isPersonalInfoRequested || isMissing);
        }
    }
}
