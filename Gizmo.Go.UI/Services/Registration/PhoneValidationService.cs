using PhoneNumbers;

namespace Gizmo.Go.UI.Services.Registration
{
    public sealed class PhoneValidationService : IPhoneValidationService
    {
        private static readonly PhoneNumberUtil _util = PhoneNumberUtil.GetInstance();
        private const int DefaultMaxDigits = 15;

        public PhoneValidationResult Validate(string input, string regionCode)
        {
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(regionCode))
                return PhoneValidationResult.Invalid;

            try
            {
                var parsed = _util.Parse(input, regionCode.ToUpperInvariant());
                if (!_util.IsValidNumber(parsed))
                    return PhoneValidationResult.Invalid;

                return new PhoneValidationResult
                {
                    IsValid = true,
                    E164 = _util.Format(parsed, PhoneNumberFormat.E164),
                    FormattedNational = FormatNationalLocal(parsed, regionCode)
                };
            }
            catch (NumberParseException)
            {
                return PhoneValidationResult.Invalid;
            }
        }

        public int GetMaxLength(string regionCode)
        {
            if (string.IsNullOrWhiteSpace(regionCode))
                return DefaultMaxDigits;

            var metadata = _util.GetMetadataForRegion(regionCode.ToUpperInvariant());
            return metadata?.GeneralDesc.PossibleLengthList.Count > 0
                ? metadata.GeneralDesc.PossibleLengthList.Max()
                : DefaultMaxDigits;
        }

        public string MaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return string.Empty;

            var digits = new string(phone.Where(char.IsDigit).ToArray());
            if (digits.Length < 6)
                return new string('*', digits.Length);

            var prefix = phone.StartsWith('+') ? "+" : string.Empty;
            var first4 = digits[..4];
            var last2 = digits[^2..];

            return $"{prefix}{first4[0]} {first4[1..]} ***-**-{last2}";
        }

        private static string FormatNationalLocal(PhoneNumber parsed, string iso2)
        {
            var national = _util.Format(parsed, PhoneNumberFormat.NATIONAL);
            var ndd = _util.GetNddPrefixForRegion(iso2.ToUpperInvariant(), true);

            if (!string.IsNullOrEmpty(ndd) && national.StartsWith(ndd))
                national = national[ndd.Length..].TrimStart();

            return national;
        }
    }
}
