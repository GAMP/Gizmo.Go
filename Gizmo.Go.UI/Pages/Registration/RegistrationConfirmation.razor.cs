using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationConfirmation : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        private RegistrationConfirmationViewState RegistrationConfirmationViewState { get; set; } = null!;

        [Inject]
        private RegistrationConfirmationViewService RegistrationConfirmationViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region FIELDS

        private ElementReference[] _inputRefs = [];

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationConfirmationViewState);
            _inputRefs = new ElementReference[RegistrationConfirmationViewState.Digits.Length];
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            var length = RegistrationConfirmationViewState.Digits.Length;
            if (_inputRefs.Length != length)
                _inputRefs = new ElementReference[length];
        }

        #endregion

        #region METHODS

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationConfirmationViewState);
        }

        private async Task OnDigitInput(int index, ChangeEventArgs e)
        {
            await RegistrationConfirmationViewService.SetDigitAsync(index, e.Value?.ToString() ?? string.Empty);

            var nextIndex = index + 1;
            if (RegistrationConfirmationViewState.Digits[index].Length == 1
                && nextIndex < RegistrationConfirmationViewState.Digits.Length
                && nextIndex < _inputRefs.Length)
            {
                await Task.Yield();
                await SafeFocusAsync(_inputRefs[nextIndex]);
            }
        }

        private async Task OnDigitKeyDown(int index, KeyboardEventArgs e)
        {
            if (e.Key != "Backspace") return;

            if (RegistrationConfirmationViewState.Digits[index].Length > 0)
            {
                await RegistrationConfirmationViewService.ClearDigitAsync(index);
                return;
            }

            if (index > 0)
            {
                await RegistrationConfirmationViewService.ClearDigitAsync(index - 1);
                await Task.Yield();
                await SafeFocusAsync(_inputRefs[index - 1]);
            }
        }

        private async Task ConfirmAsync() => await RegistrationConfirmationViewService.ConfirmAsync();

        private async Task NavigateBack() => await RegistrationConfirmationViewService.NavigateBackAsync();

        private async Task RestartTimer() => await RegistrationConfirmationViewService.RestartTimerAsync();

        private async Task SafeFocusAsync(ElementReference elementReference)
        {
            try
            {
                await elementReference.FocusAsync();
            }
            catch (JSDisconnectedException) { }
            catch (InvalidOperationException) { }
        }

        #endregion
    }
}
