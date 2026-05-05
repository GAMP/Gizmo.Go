namespace Gizmo.Go.UI.Services.Registration
{
    public sealed class RegistrationSessionService : IRegistrationSessionService
    {
        private RegistrationSnapshot _state = new();

        public event EventHandler? Changed;

        public RegistrationSnapshot State => _state;

        public string Token => _state.Token;
        public string Password => _state.Password;
        public string Phone => _state.Phone;
        public string Email => _state.Email;
        public int CodeLength => _state.CodeLength;
        public bool HasToken => _state.Token.Length > 0;
        public bool HasSession => HasToken && _state.Password.Length > 0;
        public RegistrationFlow Flow => _state.Flow;

        public void SetFlow(RegistrationFlow flow)
        {
            _state = _state with { Flow = flow };
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void SetToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token must not be null or empty.", nameof(token));

            _state = _state with { Token = token };
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void SetPassword(string password)
        {
            _state = _state with { Password = password ?? string.Empty };
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void SetPhone(string phone)
        {
            _state = _state with { Phone = phone ?? string.Empty };
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void SetEmail(string email)
        {
            _state = _state with { Email = email ?? string.Empty };
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void SetCodeLength(int codeLength)
        {
            _state = _state with { CodeLength = codeLength };
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            _state = new();
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
