using System.Linq.Expressions;
using Gizmo.Go.Core.Models;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Gizmo.Go.UI.Components
{
    /// <summary>
    /// Labeled sex select of the auth screens with the validation message of the bound property.
    /// Used by the registration profile step and the profile completion page.
    /// </summary>
    public partial class SexSelectField : ComponentBase
    {
        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        [CascadingParameter] private EditContext? EditContext { get; set; }

        [Parameter] public string Id { get; set; } = "sex";
        [Parameter] public string Label { get; set; } = string.Empty;
        [Parameter] public UserSex Value { get; set; }
        [Parameter] public EventCallback<UserSex> ValueChanged { get; set; }

        /// <summary>
        /// Bound property of the view state: source of the validation message and the invalid state.
        /// </summary>
        [Parameter] public Expression<Func<UserSex>>? For { get; set; }
        [Parameter] public bool Disabled { get; set; }

        private bool IsInvalid =>
            EditContext is not null && For is not null && EditContext.GetValidationMessages(FieldIdentifier.Create(For)).Any();

        private Task OnChangedAsync(ChangeEventArgs e)
        {
            var sex = Enum.TryParse<UserSex>(e.Value?.ToString(), out var parsed) ? parsed : UserSex.Unspecified;
            return ValueChanged.InvokeAsync(sex);
        }
    }
}
