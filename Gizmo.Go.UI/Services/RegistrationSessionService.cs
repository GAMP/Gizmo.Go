namespace Gizmo.Go.UI.Services
{
    public sealed class RegistrationSessionService : IRegistrationSessionService
    {
        private string _token = string.Empty;
        private string _password = string.Empty;

        public string Token => _token;
        public string Password => _password;
        public bool HasToken => _token.Length > 0;
        public bool HasSession => HasToken && _password.Length > 0;

        public void SetToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token must not be null or empty.", nameof(token));

            _token = token;
        }

        public void SetPassword(string password)
        {
            _password = password ?? string.Empty;
        }

        public void Clear()
        {
            _token = string.Empty;
            _password = string.Empty;
        }
    }
}
