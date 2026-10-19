namespace Gizmo.Go.Provider.Direct.Helpers
{
    internal static class ContactMasking
    {
        internal static string MaskEmail(string email)
        {
            var at = email.IndexOf('@');
            if (at <= 1) return email;
            return email[0] + "***" + email[at..];
        }

        internal static string MaskPhone(string phone)
        {
            var digits = new string(phone.TrimStart('+').Where(char.IsDigit).ToArray());
            if (digits.Length <= 4) return digits;
            return "****" + digits[^4..];
        }
    }
}
