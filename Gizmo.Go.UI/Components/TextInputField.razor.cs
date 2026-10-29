using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Gizmo.Go.UI.Components
{
    /// <summary>
    /// Labeled text input of the auth screens with the validation message of the bound property.
    /// Used by the registration profile steps and the profile completion page.
    /// </summary>
    public partial class TextInputField : ComponentBase
    {
        [CascadingParameter] private EditContext? EditContext { get; set; }

        [Parameter] public string Id { get; set; } = "text";
        [Parameter] public string Label { get; set; } = string.Empty;
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string?> ValueChanged { get; set; }

        /// <summary>
        /// Bound property of the view state: source of the validation message and the invalid state.
        /// </summary>
        [Parameter] public Expression<Func<string?>>? For { get; set; }
        [Parameter] public string InputType { get; set; } = "text";
        [Parameter] public string? Autocomplete { get; set; }
        [Parameter] public string? InputMode { get; set; }
        [Parameter] public int? MaxLength { get; set; }
        [Parameter] public string? Placeholder { get; set; }
        [Parameter] public bool Disabled { get; set; }

        private bool IsInvalid =>
            EditContext is not null && For is not null && EditContext.GetValidationMessages(FieldIdentifier.Create(For)).Any();

        private Task OnInputAsync(ChangeEventArgs e) => ValueChanged.InvokeAsync(e.Value?.ToString());
    }
}
