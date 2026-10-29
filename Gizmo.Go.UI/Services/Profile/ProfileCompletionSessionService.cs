using Gizmo.Go.Core.Models.PhoneValidation;
using Gizmo.Go.Core.Models.Profile;

namespace Gizmo.Go.UI.Services.Profile
{
    public sealed class ProfileCompletionSessionService : IProfileCompletionSessionService
    {
        public event EventHandler? Changed;

        public bool IsActive { get; private set; }

        public ProfileCompletionFields Fields { get; private set; } = ProfileCompletionFields.None;

        public UserProfileDetails? Profile { get; private set; }

        public IReadOnlyList<PhoneCountry> PhoneCountries { get; private set; } = Array.Empty<PhoneCountry>();

        public string? DefaultPhoneRegionCode { get; private set; }

        public string LandingUrl { get; private set; } = string.Empty;

        public void Start(ProfileCompletionFields fields, UserProfileDetails profile, IReadOnlyList<PhoneCountry> phoneCountries, string? defaultPhoneRegionCode, string landingUrl)
        {
            Fields = fields;
            Profile = profile;
            PhoneCountries = phoneCountries;
            DefaultPhoneRegionCode = defaultPhoneRegionCode;
            LandingUrl = landingUrl;
            IsActive = true;

            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            Fields = ProfileCompletionFields.None;
            Profile = null;
            PhoneCountries = Array.Empty<PhoneCountry>();
            DefaultPhoneRegionCode = null;
            LandingUrl = string.Empty;
            IsActive = false;

            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
