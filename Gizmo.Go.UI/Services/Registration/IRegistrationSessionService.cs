using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Models.Registration;

namespace Gizmo.Go.UI.Services.Registration
{
    public interface IRegistrationSessionService
    {
        event EventHandler? Changed;
        event EventHandler? Cleared;

        string Token { get; }
        string Destination { get; }
        int CodeLength { get; }
        int ExpiresInSeconds { get; }
        RegistrationFlow Flow { get; }

        string? ActualContact { get; }
        string? Country { get; }

        /// <summary>
        /// Gets a value indicating that the mobile phone was already confirmed by the registration provider
        /// and must not be asked for again.
        /// </summary>
        bool HasConfirmedMobilePhone { get; }

        string Username { get; }
        string? Password { get; }
        string? FirstName { get; }
        string? LastName { get; }
        DateTime? BirthDate { get; }
        UserSex Sex { get; }
        string? Email { get; }
        string? MobilePhone { get; }
        string? Phone { get; }
        string? PhoneE164 { get; }

        bool ShowAllProviders { get; }
        bool AgreementsAccepted { get; }
        IReadOnlyList<RegistrationAgreementChoice> AgreementChoices { get; }

        RegistrationProvider? SelectedProvider { get; }
        RequiredUserInfo? RequiredUserInfo { get; }
        Guid? FailedProviderChannelGuid { get; }

        /// <summary>
        /// Gets the contact the completed registration is bound to (phone, otherwise email).
        /// Kept by <see cref="Clear"/> for the success screen, which resets it.
        /// </summary>
        string? CompletedContact { get; }

        /// <summary>
        /// Gets a value indicating that <see cref="CompletedContact"/> is an email address.
        /// </summary>
        bool IsCompletedContactEmail { get; }

        void SetStartResult(string token, string destination, int codeLength, int expiresInSeconds, RegistrationFlow flow);
        void SetContactDetails(string? actualContact, string? country = null);
        void SetProfileBasics(string username, string? password,
            string? firstName, string? lastName,
            DateTime? birthDate, UserSex sex, string? email);
        void SetMobilePhone(string? mobilePhone);
        void SetPhone(string? phone);
        void SetPhoneE164(string? e164);
        void SetShowAllProviders(bool value);
        void SetAgreementsAccepted(bool value);
        void SetAgreementChoices(IEnumerable<RegistrationAgreementChoice> choices);
        void SetSelectedProvider(RegistrationProvider? provider);
        void SetRequiredUserInfo(RequiredUserInfo? info);
        void SetFailedProviderChannelGuid(Guid? channelGuid);
        void SetCompletedContact(string? contact, bool isEmail);
        void Clear();
    }
}
