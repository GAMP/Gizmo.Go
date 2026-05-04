namespace Gizmo.Go.UI.Services.Registration
{
    public interface IRegistrationSessionService
    {
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
