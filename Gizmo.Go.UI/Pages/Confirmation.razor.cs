using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Go.UI.Pages
{
    public partial class Confirmation : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        private ConfirmationViewState ConfirmationViewState { get; set; } = null!;

        [Inject]
        private ConfirmationViewService ConfirmationViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region FIELDS

        private readonly ElementReference[] _inputRefs = new ElementReference[6];

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(ConfirmationViewState);
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        public void Dispose()
        {
            this.UnsubscribeChange(ConfirmationViewState);
        }

        private async Task OnDigitInput(int index, ChangeEventArgs e)
        {
            await ConfirmationViewService.SetDigitAsync(index, e.Value?.ToString() ?? string.Empty);

            if (ConfirmationViewState.Digits[index].Length == 1 && index < 5)
            {
                await Task.Yield();
                await SafeFocusAsync(_inputRefs[index + 1]);
            }
        }

        private async Task OnDigitKeyDown(int index, KeyboardEventArgs e)
        {
            if (e.Key == "Backspace" && ConfirmationViewState.Digits[index].Length == 0 && index > 0)
            {
                await ConfirmationViewService.ClearDigitAsync(index - 1);
                await Task.Yield();
                await SafeFocusAsync(_inputRefs[index - 1]);
            }
        }

        private async Task ConfirmAsync() => await ConfirmationViewService.ConfirmAsync();

        private async Task NavigateBack() => await ConfirmationViewService.NavigateBackAsync();

        private async Task RestartTimer() => await ConfirmationViewService.RestartTimerAsync();

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
