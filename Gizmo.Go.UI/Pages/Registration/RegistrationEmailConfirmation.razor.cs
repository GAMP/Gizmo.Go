using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationEmailConfirmation : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        private RegistrationEmailConfirmationViewState RegistrationEmailConfirmationViewState { get; set; } = null!;

        [Inject]
        private RegistrationEmailConfirmationViewService RegistrationEmailConfirmationViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region FIELDS

        private ElementReference[] _inputRefs = [];

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationEmailConfirmationViewState);
            _inputRefs = new ElementReference[RegistrationEmailConfirmationViewState.Digits.Length];
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            var len = RegistrationEmailConfirmationViewState.Digits.Length;
            if (_inputRefs.Length != len)
                _inputRefs = new ElementReference[len];
            base.OnParametersSet();
        }

        #endregion

        #region METHODS

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationEmailConfirmationViewState);
        }

        private async Task OnDigitInput(int index, ChangeEventArgs e)
        {
            await RegistrationEmailConfirmationViewService.SetDigitAsync(index, e.Value?.ToString() ?? string.Empty);

            if (RegistrationEmailConfirmationViewState.Digits[index].Length == 1 && index < RegistrationEmailConfirmationViewState.Digits.Length - 1)
            {
                await Task.Yield();
                await SafeFocusAsync(_inputRefs[index + 1]);
            }
        }

        private async Task OnDigitKeyDown(int index, KeyboardEventArgs e)
        {
            if (e.Key != "Backspace") return;

            if (RegistrationEmailConfirmationViewState.Digits[index].Length > 0)
            {
                await RegistrationEmailConfirmationViewService.ClearDigitAsync(index);
                return;
            }

            if (index > 0)
            {
                await RegistrationEmailConfirmationViewService.ClearDigitAsync(index - 1);
                await Task.Yield();
                await SafeFocusAsync(_inputRefs[index - 1]);
            }
        }

        private async Task ConfirmAsync() => await RegistrationEmailConfirmationViewService.ConfirmAsync();

        private async Task NavigateBack() => await RegistrationEmailConfirmationViewService.NavigateBackAsync();

        private async Task RestartTimer() => await RegistrationEmailConfirmationViewService.RestartTimerAsync();

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
