namespace Gizmo.Go.UI.Services
{
    public interface IRegistrationSessionService
    {
        string Token { get; }
        string Password { get; }
        bool HasToken { get; }
        bool HasSession { get; }

        void SetToken(string token);
        void SetPassword(string password);
        void Clear();
    }
}
