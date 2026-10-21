using Gizmo.Go.Core.Models.PasswordRecovery;

namespace Gizmo.Go.UI.Services.PasswordRecovery
{
    public interface IPasswordRecoverySessionService
    {
        event EventHandler? Changed;

        PasswordRecoveryProvider? ActiveProvider { get; }

        string MatchValue { get; }

        PasswordRecoveryIdentifierKind? IdentifierKind { get; }

        IReadOnlyList<PasswordRecoveryProvider> AvailableMethods { get; }

        string Token { get; }

        string Destination { get; }

        string RedirectUrl { get; }

        string CallPhoneNumber { get; }

        int CodeLength { get; }

        int ExpiresInSeconds { get; }

        PasswordRecoveryAction Action { get; }

        bool IsTokenConfirmed { get; }

        bool IsCodeConfirmed { get; }

        Guid? FailedProviderChannelGuid { get; }

        bool ShowAllProviders { get; }

        void SetActiveProvider(PasswordRecoveryProvider provider);

        void SetIdentifierKind(PasswordRecoveryIdentifierKind identifierKind);

        void SetAvailableMethods(IReadOnlyList<PasswordRecoveryProvider> methods);

        void SetMatchValue(string value, PasswordRecoveryIdentifierKind identifierKind);

        void SetStartResult(string token, string destination, int codeLength, int expiresInSeconds);

        void SetRedirectStartResult(string token, string redirectUrl, int expiresInSeconds);

        void SetCallStartResult(string token, string phoneNumber, int expiresInSeconds);

        void SetTokenConfirmed(bool value);

        void SetCodeConfirmed(bool value);

        void SetFailedProviderChannelGuid(Guid? channelGuid);

        void SetShowAllProviders(bool value);

        /// <summary>
        /// Result of the account search on the identifier entry step; null until the search ran.
        /// The server answers an unknown account and an account without usable methods alike (empty method list).
        /// </summary>
        bool? IsAccountFound { get; }

        void SetAccountFound(bool value);

        /// <summary>
        /// Drops the account search and the started recovery; keeps the chosen identifier kind.
        /// </summary>
        void ResetSearch();

        /// <summary>
        /// Drops the started recovery (token, confirmation, active provider) and keeps the entered identifier and available methods.
        /// </summary>
        void ResetStart();

        void Clear();
    }
}
