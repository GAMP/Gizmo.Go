namespace Gizmo.Go.UI.Services.Registration
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
