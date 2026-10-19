using Gizmo.Go.Core.Models.PhoneValidation;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components
{
    public partial class PhoneInputField : ComponentBase, IDisposable
    {
        [Parameter] public IReadOnlyList<PhoneCountry> Countries { get; set; } = Array.Empty<PhoneCountry>();

        /// <summary>
        /// Phone value in the client format: calling code digits followed by the national number digits.
        /// </summary>
        [Parameter] public string Value { get; set; } = "";
        [Parameter] public EventCallback<string> ValueChanged { get; set; }
        [Parameter] public string? RegionCode { get; set; }
        [Parameter] public EventCallback<string> RegionCodeChanged { get; set; }
        [Parameter] public string CountryPlaceholder { get; set; } = "";
        [Parameter] public bool IsInvalid { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public string Id { get; set; } = "phone";

        private string _phoneInputValue = "";
        private CancellationTokenSource? _debounceCts;

        private PhoneCountry? SelectedCountry =>
            string.IsNullOrEmpty(RegionCode)
                ? null
                : Countries.FirstOrDefault(c => string.Equals(c.RegionCode, RegionCode, StringComparison.OrdinalIgnoreCase));

        private int MaxLength =>
            SelectedCountry is { } country
                ? Math.Max(country.MaxLength, country.Placeholder?.Length ?? 0)
                : 15;

        private string CallingCodeDigits => ToDigits(SelectedCountry?.CallingCode);

        protected override void OnParametersSet()
        {
            if (_debounceCts is null)
            {
                var national = ToNationalInput(Value);
                if (_phoneInputValue != national)
                    _phoneInputValue = national;
            }
        }

        private async Task OnCountrySelected(ChangeEventArgs e)
        {
            var regionCode = e.Value?.ToString() ?? "";
            var country = Countries.FirstOrDefault(c => string.Equals(c.RegionCode, regionCode, StringComparison.OrdinalIgnoreCase));

            _phoneInputValue = "";
            await RegionCodeChanged.InvokeAsync(regionCode);
            await ValueChanged.InvokeAsync(ToDigits(country?.CallingCode));
        }

        private async Task OnPhoneInputAsync(ChangeEventArgs e)
        {
            _debounceCts?.Cancel();
            _debounceCts = new CancellationTokenSource();
            _phoneInputValue = e.Value?.ToString() ?? "";
            try
            {
                await Task.Delay(150, _debounceCts.Token);
                await ValueChanged.InvokeAsync(CallingCodeDigits + ToDigits(_phoneInputValue));
            }
            catch (TaskCanceledException) { }
        }

        private async Task OnPhoneFormattedAsync(ChangeEventArgs e)
        {
            _phoneInputValue = e.Value?.ToString() ?? "";
            await ValueChanged.InvokeAsync(CallingCodeDigits + ToDigits(_phoneInputValue));
        }

        private string ToNationalInput(string value)
        {
            var digits = ToDigits(value);
            var code = CallingCodeDigits;

            if (code.Length > 0 && digits.StartsWith(code, StringComparison.Ordinal))
                digits = digits[code.Length..];

            // keep the text the user typed while it carries the same digits
            return ToDigits(_phoneInputValue) == digits ? _phoneInputValue : digits;
        }

        private static string ToDigits(string? value) =>
            new((value ?? string.Empty).Where(char.IsDigit).ToArray());

        public void Dispose()
        {
            _debounceCts?.Cancel();
            _debounceCts?.Dispose();
        }
    }
}
