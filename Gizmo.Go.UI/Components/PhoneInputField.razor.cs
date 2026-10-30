using System.Globalization;
using Gizmo.Go.Core.Models.PhoneValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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
        [Parameter] public bool IsInvalid { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public string Id { get; set; } = "phone";

        /// <summary>
        /// Country preselected when no region is chosen (ISO 3166-1 alpha-2); otherwise the first country.
        /// It has to be set before the countries arrive: the preselection is applied once.
        /// </summary>
        [Parameter] public string? DefaultRegionCode { get; set; }

        private string _phoneInputValue = "";
        private string _lastEmittedValue = "";
        private string _regionCode = "";
        private string? _lastRegionCodeParameter;
        private bool _debouncePending;
        private bool _isOpen;
        private bool _defaultApplied;
        private IReadOnlyList<PhoneCountry>? _countriesSource;
        private IReadOnlyList<PhoneCountry> SortedCountries { get; set; } = Array.Empty<PhoneCountry>();
        private CancellationTokenSource? _debounceCts;

        private bool IsCountryDisabled => Disabled || SortedCountries.Count == 0;

        private PhoneCountry? SelectedCountry =>
            string.IsNullOrEmpty(_regionCode)
                ? null
                : SortedCountries.FirstOrDefault(c => string.Equals(c.RegionCode, _regionCode, StringComparison.OrdinalIgnoreCase));

        private int MaxLength =>
            SelectedCountry is { } country
                ? Math.Max(country.MaxLength, country.Placeholder?.Length ?? 0)
                : 15;

        private string CallingCodeDigits => ToDigits(SelectedCountry?.CallingCode);

        protected override void OnParametersSet()
        {
            if (!ReferenceEquals(_countriesSource, Countries))
            {
                _countriesSource = Countries;
                SortedCountries = Countries
                    .OrderBy(c => c.CountryName, StringComparer.Create(CultureInfo.CurrentUICulture, true))
                    .ToList();
                _defaultApplied = false;
            }

            if (_lastRegionCodeParameter != RegionCode)
            {
                _lastRegionCodeParameter = RegionCode;
                _regionCode = RegionCode ?? "";
            }

            if (!_debouncePending)
            {
                var national = ToNationalInput(Value);
                if (_phoneInputValue != national)
                    _phoneInputValue = national;

                _lastEmittedValue = Value ?? "";
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (_defaultApplied || SortedCountries.Count == 0)
                return;

            _defaultApplied = true;

            if (string.IsNullOrEmpty(_regionCode))
            {
                // Preselect the server default country (mobile options), otherwise
                // the first one. Value is left untouched so that no validation error is shown before
                // the user types a number.
                var defaultCountry = SortedCountries.FirstOrDefault(c =>
                    string.Equals(c.RegionCode, DefaultRegionCode, StringComparison.OrdinalIgnoreCase));
                _regionCode = (defaultCountry ?? SortedCountries[0]).RegionCode;
                await RegionCodeChanged.InvokeAsync(_regionCode);
            }
        }

        private void Toggle()
        {
            if (IsCountryDisabled)
                return;

            _isOpen = !_isOpen;
        }

        private void Close() => _isOpen = false;

        private void OnKeyDown(KeyboardEventArgs e)
        {
            if (_isOpen && e.Key == "Escape")
                Close();
        }

        private async Task OnOptionKeyDown(KeyboardEventArgs e, PhoneCountry country)
        {
            if (e.Key is "Enter" or " ")
                await SelectCountryAsync(country);
        }

        private async Task SelectCountryAsync(PhoneCountry country)
        {
            _isOpen = false;

            if (string.Equals(country.RegionCode, _regionCode, StringComparison.OrdinalIgnoreCase))
                return;

            _debounceCts?.Cancel();
            _debouncePending = false;
            _regionCode = country.RegionCode;
            _phoneInputValue = "";

            await RegionCodeChanged.InvokeAsync(country.RegionCode);
            await EmitValueAsync(ToDigits(country.CallingCode));
        }

        private async Task OnPhoneInputAsync(ChangeEventArgs e)
        {
            _debounceCts?.Cancel();
            var cts = new CancellationTokenSource();
            _debounceCts = cts;
            _debouncePending = true;

            // only digits and number formatting characters: letters are removed from the field right away
            var raw = e.Value?.ToString() ?? "";
            var clean = SanitizeInput(raw);
            if (clean != raw)
            {
                // render the raw text first so the renderer sees a change and writes the cleaned value back
                _phoneInputValue = raw;
                StateHasChanged();
                await Task.Yield();
            }

            _phoneInputValue = clean;
            try
            {
                await Task.Delay(150, cts.Token);
                _debouncePending = false;
                await EmitValueAsync(CallingCodeDigits + ToDigits(_phoneInputValue));
            }
            catch (TaskCanceledException) { }
        }

        private async Task OnPhoneFormattedAsync(ChangeEventArgs e)
        {
            _debounceCts?.Cancel();
            _debouncePending = false;
            _phoneInputValue = SanitizeInput(e.Value?.ToString() ?? "");
            await EmitValueAsync(CallingCodeDigits + ToDigits(_phoneInputValue));
        }

        private static string SanitizeInput(string value) =>
            new(value.Where(c => char.IsDigit(c) || c is ' ' or '-' or '(' or ')').ToArray());

        // The same value is never emitted twice in a row: a repeated emit restarts the async
        // validation in the view service and temporarily disables the submit button.
        private async Task EmitValueAsync(string value)
        {
            if (value == _lastEmittedValue)
                return;

            _lastEmittedValue = value;
            await ValueChanged.InvokeAsync(value);
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
