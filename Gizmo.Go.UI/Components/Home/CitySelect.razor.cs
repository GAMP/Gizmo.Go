using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Home
{
    /// <summary>
    /// City selector of the clubs list. With a single city it is a plain label without the dropdown.
    /// </summary>
    public partial class CitySelect : ComponentBase
    {
        // purely visual state of the dropdown
        private bool _open;

        [Parameter]
        public IReadOnlyList<string> Cities { get; set; } = [];

        [Parameter]
        public string? SelectedCity { get; set; }

        [Parameter]
        public EventCallback<string> OnSelect { get; set; }

        private void Toggle() => _open = !_open;

        private async Task SelectAsync(string city)
        {
            _open = false;
            await OnSelect.InvokeAsync(city);
        }
    }
}
