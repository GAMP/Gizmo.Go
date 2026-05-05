using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components
{
    public partial class PhoneInputField : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Parameter] public string Value { get; set; } = "";
        [Parameter] public EventCallback<string> ValueChanged { get; set; }
        [Parameter] public string CountryIso2 { get; set; } = "";
        [Parameter] public EventCallback<string> CountryIso2Changed { get; set; }
        [Parameter] public string Placeholder { get; set; } = "";
        [Parameter] public string CountryPlaceholder { get; set; } = "";
        [Parameter] public string DialCode { get; set; } = "";
        [Parameter] public int MaxLength { get; set; } = 15;
        [Parameter] public bool IsInvalid { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public string Id { get; set; } = "phone";

        #endregion

        #region FIELDS

        private string _phoneInputValue = "";
        private CancellationTokenSource? _debounceCts;

        #endregion

        #region OVERRIDES

        protected override void OnParametersSet()
        {
            if (_phoneInputValue != Value && _debounceCts is null)
                _phoneInputValue = Value;
        }

        #endregion

        #region METHODS

        private async Task OnCountrySelected(ChangeEventArgs e)
        {
            var iso = e.Value?.ToString() ?? "";
            _phoneInputValue = "";
            await CountryIso2Changed.InvokeAsync(iso);
            await ValueChanged.InvokeAsync(_phoneInputValue);
        }

        private async Task OnPhoneInputAsync(ChangeEventArgs e)
        {
            _debounceCts?.Cancel();
            _debounceCts = new CancellationTokenSource();
            _phoneInputValue = e.Value?.ToString() ?? "";
            try
            {
                await Task.Delay(150, _debounceCts.Token);
                await ValueChanged.InvokeAsync(_phoneInputValue);
            }
            catch (TaskCanceledException) { }
        }

        private async Task OnPhoneFormattedAsync(ChangeEventArgs e)
        {
            _phoneInputValue = e.Value?.ToString() ?? "";
            await ValueChanged.InvokeAsync(_phoneInputValue);
        }

        public void Dispose()
        {
            _debounceCts?.Cancel();
            _debounceCts?.Dispose();
        }

        #endregion
    }
}
