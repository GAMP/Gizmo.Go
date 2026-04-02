using System.Linq.Expressions;
using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Go.UI.Pages
{
    public partial class CreateAccount : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        private CreateAccountViewState CreateAccountViewState { get; set; } = null!;

        [Inject]
        private CreateAccountViewService CreateAccountViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region FIELDS

        private string _selectedCountryIso2 = string.Empty;
        
        private string _phoneInputValue = string.Empty;
        
        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(CreateAccountViewState);
            _selectedCountryIso2 = CreateAccountViewState.SelectedCountryIso2;
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);

            return CreateAccountViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private async Task OnCountryChangedAsync(ChangeEventArgs args)
        {
            _selectedCountryIso2 = args.Value?.ToString() ?? string.Empty;
            _phoneInputValue = string.Empty;
            await CreateAccountViewService.UpdatePhoneAsync(_phoneInputValue, _selectedCountryIso2);
        }

        private async Task OnPhoneInputAsync(ChangeEventArgs args)
        {
            _phoneInputValue = args.Value?.ToString() ?? string.Empty;
            await CreateAccountViewService.UpdatePhoneAsync(_phoneInputValue, _selectedCountryIso2);
            
        }

        private async Task OnPhoneFormattedAsync(ChangeEventArgs args)
        {
            _phoneInputValue = args.Value?.ToString() ?? string.Empty;
            await CreateAccountViewService.UpdatePhoneAsync(_phoneInputValue, _selectedCountryIso2);
            
            if (!string.IsNullOrEmpty(CreateAccountViewState.FormattedPhoneInput))
                _phoneInputValue = CreateAccountViewState.FormattedPhoneInput;
        }
        
        private async Task ToggleTerms() => await CreateAccountViewService.ToggleTermsAsync(); //  что за Terms откуда и для чего нужен?

        private void OnTermsKeyDown(KeyboardEventArgs args)
        {
            if (args.Key is " " or "Enter")
                _ = CreateAccountViewService.ToggleTermsAsync();
        }

        private async Task SubmitAsync() => await CreateAccountViewService.SubmitAsync();

        private async Task NavigateToLogin() => await CreateAccountViewService.NavigateBackAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(CreateAccountViewState);
        }

        #endregion
    }
}
