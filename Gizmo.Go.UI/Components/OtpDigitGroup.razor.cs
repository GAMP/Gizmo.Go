using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Go.UI.Components
{
    public partial class OtpDigitGroup : ComponentBase
    {
        [Parameter] public string[] Digits { get; set; } = [];
        [Parameter] public EventCallback<(int Index, string Value)> OnDigitChanged { get; set; }
        [Parameter] public EventCallback<int> OnDigitCleared { get; set; }
        [Parameter] public string AriaGroupLabel { get; set; } = "";
        [Parameter] public Func<int, string>? AriaLabelForIndex { get; set; }

        private ElementReference[] _inputRefs = [];

        protected override void OnParametersSet()
        {
            if (_inputRefs.Length != Digits.Length)
                _inputRefs = new ElementReference[Digits.Length];
        }

        private async Task OnInput(int index, ChangeEventArgs e)
        {
            var raw = e.Value?.ToString() ?? "";
            await OnDigitChanged.InvokeAsync((index, raw));

            var nextIndex = index + 1;
            if (Digits[index].Length == 1 && nextIndex < Digits.Length)
            {
                await Task.Yield();
                await SafeFocusAsync(_inputRefs[nextIndex]);
            }
        }

        private async Task OnKeyDown(int index, KeyboardEventArgs e)
        {
            if (e.Key != "Backspace") return;

            if (Digits[index].Length > 0)
            {
                await OnDigitCleared.InvokeAsync(index);
                return;
            }
            if (index > 0)
            {
                await OnDigitCleared.InvokeAsync(index - 1);
                await Task.Yield();
                await SafeFocusAsync(_inputRefs[index - 1]);
            }
        }

        private async Task SafeFocusAsync(ElementReference er)
        {
            try { await er.FocusAsync(); }
            catch (JSDisconnectedException) { }
            catch (InvalidOperationException) { }
        }
    }
}
