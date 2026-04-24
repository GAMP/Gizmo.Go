namespace Gizmo.Go.UI.Services.Registration
{
    public sealed class RegistrationSessionService : IRegistrationSessionService
    {
        private string _token = string.Empty;
        private string _password = string.Empty;
        private string _phone = string.Empty;
        private string _email = string.Empty;
        private int _codeLength;

        public string Token => _token;
        public string Password => _password;
        public string Phone => _phone;
        public string Email => _email;
        public int CodeLength => _codeLength;
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

        public void SetPhone(string phone)
        {
            _phone = phone ?? string.Empty;
        }

        public void SetEmail(string email)
        {
            _email = email ?? string.Empty;
        }

        public void SetCodeLength(int codeLength)
        {
            _codeLength = codeLength;
        }

        public void Clear()
        {
            _token = string.Empty;
            _password = string.Empty;
            _phone = string.Empty;
            _email = string.Empty;
            _codeLength = 0;
        }
    }
}
