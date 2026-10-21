using Gizmo.Go.Core.Models.PhoneValidation;

namespace Gizmo.Go.UI.Services.Common
{
    public static class ContactMasking
    {
        public static string MaskEmail(string email)
        {
            var at = email.IndexOf('@');
            if (at <= 1) return email;
            return email[0] + "***" + email[at..];
        }

        /// <summary>
        /// Masks a phone as in Figma "+7 999 ***-**-42": calling code, the first 3 national digits,
        /// masked middle and the last 2 digits. The calling code is the longest one from <paramref name="countries"/>
        /// that the number starts with; without a match the number is masked without a code.
        /// </summary>
        public static string MaskPhone(string phone, IReadOnlyList<PhoneCountry> countries)
        {
            var digits = new string(phone.Where(char.IsDigit).ToArray());

            var callingCode = countries
                .Select(c => new string(c.CallingCode.Where(char.IsDigit).ToArray()))
                .Where(code => code.Length > 0 && digits.Length > code.Length && digits.StartsWith(code, StringComparison.Ordinal))
                .OrderByDescending(code => code.Length)
                .FirstOrDefault() ?? string.Empty;

            var national = digits[callingCode.Length..];
            var prefix = callingCode.Length > 0 ? "+" + callingCode + " " : string.Empty;

            if (national.Length <= 5)
                return prefix + new string('*', Math.Max(national.Length - 2, 0)) + national[Math.Max(national.Length - 2, 0)..];

            // tail = masked digits + last 2 digits, grouped from the end as "***-**-42"
            var tail = new string('*', national.Length - 5) + national[^2..];
            var groups = tail.Length > 4
                ? new[] { tail[..^4], tail[^4..^2], tail[^2..] }
                : new[] { tail[..^2], tail[^2..] };

            return prefix + national[..3] + " " + string.Join("-", groups);
        }
    }
}
