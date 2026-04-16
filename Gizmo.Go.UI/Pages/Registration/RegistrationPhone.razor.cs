using System.Linq.Expressions;
using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationPhone : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        private RegistrationPhoneViewState RegistrationPhoneViewState { get; set; } = null!;

        [Inject]
        private RegistrationPhoneViewService RegistrationPhoneViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region FIELDS

        private string _selectedCountryIso2 = string.Empty;

        private string _phoneInputValue = string.Empty;

        private CancellationTokenSource? _phoneInputDebounceCts;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationPhoneViewState);
            _selectedCountryIso2 = RegistrationPhoneViewState.SelectedCountryIso2;
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);

            return RegistrationPhoneViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private async Task OnCountryChangedAsync(ChangeEventArgs args)
        {
            _selectedCountryIso2 = args.Value?.ToString() ?? string.Empty;
            _phoneInputValue = string.Empty;
            await RegistrationPhoneViewService.UpdatePhoneAsync(_phoneInputValue, _selectedCountryIso2);
        }

        private async Task OnPhoneInputAsync(ChangeEventArgs args)
        {
            _phoneInputDebounceCts?.Cancel();
            _phoneInputDebounceCts = new CancellationTokenSource();

            _phoneInputValue = args.Value?.ToString() ?? string.Empty;

            try
            {
                await Task.Delay(150, _phoneInputDebounceCts.Token);
                await RegistrationPhoneViewService.UpdatePhoneAsync(_phoneInputValue, _selectedCountryIso2);
            }
            catch (TaskCanceledException)
            {
                // ввод продолжается — игнорируем
            }
        }

        private async Task OnPhoneFormattedAsync(ChangeEventArgs args)
        {
            _phoneInputValue = args.Value?.ToString() ?? string.Empty;
            await RegistrationPhoneViewService.UpdatePhoneAsync(_phoneInputValue, _selectedCountryIso2);

            if (!string.IsNullOrEmpty(RegistrationPhoneViewState.FormattedPhoneInput))
                _phoneInputValue = RegistrationPhoneViewState.FormattedPhoneInput;
        }

        private async Task ToggleTerms() => await RegistrationPhoneViewService.ToggleTermsAsync(); //  что за Terms откуда и для чего нужен?

        private void OnTermsKeyDown(KeyboardEventArgs args)
        {
            if (args.Key is " " or "Enter")
                _ = RegistrationPhoneViewService.ToggleTermsAsync();
        }

        private async Task SubmitAsync() => await RegistrationPhoneViewService.SubmitAsync();

        private async Task NavigateToLogin() => await RegistrationPhoneViewService.NavigateBackAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationPhoneViewState);
            _phoneInputDebounceCts?.Cancel();
            _phoneInputDebounceCts?.Dispose();
        }

        #endregion
    }
}
