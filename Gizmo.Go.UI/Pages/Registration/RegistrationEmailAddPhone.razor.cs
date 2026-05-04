using System.Linq.Expressions;
using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationEmailAddPhone : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject] private RegistrationEmailAddPhoneViewState RegistrationEmailAddPhoneViewState { get; set; } = null!;

        [Inject] private RegistrationEmailAddPhoneViewService RegistrationEmailAddPhoneViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region FIELDS

        private string _selectedCountryIso2 = string.Empty;

        private string _phoneInputValue = string.Empty;

        private CancellationTokenSource? _phoneInputDebounceCts;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationEmailAddPhoneViewState);
            _selectedCountryIso2 = RegistrationEmailAddPhoneViewState.SelectedCountryIso2;
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);

            return RegistrationEmailAddPhoneViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private async Task OnCountryChangedAsync(ChangeEventArgs args)
        {
            _selectedCountryIso2 = args.Value?.ToString() ?? string.Empty;
            _phoneInputValue = string.Empty;
            await RegistrationEmailAddPhoneViewService.UpdatePhoneAsync(_phoneInputValue, _selectedCountryIso2);
        }

        private async Task OnPhoneInputAsync(ChangeEventArgs args)
        {
            _phoneInputDebounceCts?.Cancel();
            _phoneInputDebounceCts = new CancellationTokenSource();
            _phoneInputValue = args.Value?.ToString() ?? string.Empty;

            try
            {
                await Task.Delay(150, _phoneInputDebounceCts.Token);
                await RegistrationEmailAddPhoneViewService.UpdatePhoneAsync(_phoneInputValue, _selectedCountryIso2);
            }
            catch (TaskCanceledException) { }
        }

        private async Task OnPhoneFormattedAsync(ChangeEventArgs args)
        {
            _phoneInputValue = args.Value?.ToString() ?? string.Empty;
            await RegistrationEmailAddPhoneViewService.UpdatePhoneAsync(_phoneInputValue, _selectedCountryIso2);

            if (!string.IsNullOrEmpty(RegistrationEmailAddPhoneViewState.PhoneInput))
                _phoneInputValue = RegistrationEmailAddPhoneViewState.PhoneInput;
        }

        private async Task SubmitAsync() => await RegistrationEmailAddPhoneViewService.SubmitAsync();

        private async Task NavigateBack() => await RegistrationEmailAddPhoneViewService.NavigateBackAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationEmailAddPhoneViewState);
            _phoneInputDebounceCts?.Cancel();
            _phoneInputDebounceCts?.Dispose();
        }

        #endregion
    }
}
