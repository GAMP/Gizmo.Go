using PhoneNumbers;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Gizmo.Go.UI.View.Models;

public static class PhoneCountryList
{
    private static readonly Lazy<IReadOnlyList<PhoneCountry>> _countries = new(Build);

    public static IReadOnlyList<PhoneCountry> All => _countries.Value;

    public static PhoneCountry? FindByIso2(string iso2) =>
        All.FirstOrDefault(c => c.Iso2.Equals(iso2, StringComparison.OrdinalIgnoreCase));

    private static IReadOnlyList<PhoneCountry> Build()
    {
        var util = PhoneNumberUtil.GetInstance();

        return util.GetSupportedRegions()
            .Select(iso2 => new PhoneCountry
        {
            Iso2 = iso2,
            Name = GetDisplayName(iso2),
            DialCode = "+" + util.GetCountryCodeForRegion(iso2),
            Flag = GetFlagEmoji(iso2),
            Placeholder = GetPlaceHolder(iso2)
        })
            .OrderBy(p => p.Name)
            .ToList();
    }

    private static string GetDisplayName(string iso2)
    {
        try
        {
            return new RegionInfo(iso2).DisplayName;
        }
        catch
        {
            return iso2;
        }
    }

    private static string GetFlagEmoji(string iso2)
    {
        if (iso2.Length != 2)
        {
            return string.Empty;
        }

        return string.Concat(iso2.ToUpper().Select(c => char.ConvertFromUtf32(c - 'A' + 0x1F1E6)));
    }

    private static string GetPlaceHolder(string iso2)
    {
        try
        {
            var util = PhoneNumberUtil.GetInstance(); //TODO возможно надо рассмотреть один вызов инстанса на класс
            var example = util.GetExampleNumber(iso2);
            var formatted = util.Format(example, PhoneNumberFormat.NATIONAL);
            var ndd = util.GetNddPrefixForRegion(iso2, true);

            if (!string.IsNullOrEmpty(ndd) && formatted.StartsWith(ndd))
            {
                formatted = formatted[ndd.Length..].TrimStart();
            }

            return Regex.Replace(formatted, @"\d", "#");
        }
        catch
        {
            return string.Empty;
        }
    }
}