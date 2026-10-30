using Gizmo.Go.Core.Models.PhoneValidation;
using Gizmo.Go.Core.Models.Profile;

namespace Gizmo.Go.UI.Services.Profile
{
    /// <summary>
    /// Data of the profile completion step, prepared by the post-login steps for the profile completion page.
    /// </summary>
    public interface IProfileCompletionSessionService
    {
        event EventHandler? Changed;

        /// <summary>
        /// Gets whether the step is started and not completed yet.
        /// </summary>
        bool IsActive { get; }

        ProfileCompletionFields Fields { get; }

        /// <summary>
        /// Gets the profile loaded when the step was started; null while not active.
        /// </summary>
        UserProfileDetails? Profile { get; }

        IReadOnlyList<PhoneCountry> PhoneCountries { get; }

        /// <summary>
        /// Gets the default phone country of the server (ISO 3166-1 alpha-2).
        /// </summary>
        string? DefaultPhoneRegionCode { get; }

        /// <summary>
        /// Gets where to go after the profile is saved.
        /// </summary>
        string LandingUrl { get; }

        void Start(ProfileCompletionFields fields, UserProfileDetails profile, IReadOnlyList<PhoneCountry> phoneCountries, string? defaultPhoneRegionCode, string landingUrl);

        void Clear();
    }
}
