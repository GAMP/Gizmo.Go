namespace Gizmo.Go.UI.Services.Registration
{
    public interface IRegistrationSessionService
    {
        // Raised whenever any session field changes.
        event EventHandler? Changed;

        // Atomic snapshot of the current session; intended for Changed subscribers.
        // Flat properties below are kept for backward compatibility with existing ViewService callers
        // and may be removed once all callers migrate to reading State directly.
        RegistrationSnapshot State { get; }

        string Token { get; }
        string Password { get; }
        string Phone { get; }
        string Email { get; }
        int CodeLength { get; }
        bool HasToken { get; }
        bool HasSession { get; }

        void SetToken(string token);
        void SetPassword(string password);
        void SetPhone(string phone);
        void SetEmail(string email);
        void SetCodeLength(int codeLength);
        RegistrationFlow Flow { get; }
        void SetFlow(RegistrationFlow flow);
        void Clear();
    }
}
