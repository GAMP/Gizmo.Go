using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Gizmo.Go.UI.Components
{
    /// <summary>
    /// Contact entry step shared by registration (email, phone) and password recovery (phone, email, username):
    /// title, subtitle, one field, a hint, the submit button and the optional bottom actions.
    /// Figma 159:13640, 159:13692, 159:13260, 159:13339, 159:13301.
    /// </summary>
    public partial class ContactEntryForm : ComponentBase
    {
        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public string? Subtitle { get; set; }
        [Parameter] public string? ErrorMessage { get; set; }

        /// <summary>
        /// Edit context of the page view service; the field and its validation message are rendered inside it.
        /// </summary>
        [Parameter, EditorRequired] public EditContext EditContext { get; set; } = null!;

        /// <summary>
        /// The field markup (label, input or PhoneInputField, ValidationMessage).
        /// </summary>
        [Parameter] public RenderFragment? Field { get; set; }

        /// <summary>
        /// One-line hint with an info icon under the field (registration).
        /// </summary>
        [Parameter] public string? Hint { get; set; }

        /// <summary>
        /// Hint in an info box under the field (recovery by email).
        /// </summary>
        [Parameter] public string? Info { get; set; }

        [Parameter] public string SubmitText { get; set; } = string.Empty;
        [Parameter] public bool SubmitDisabled { get; set; }
        [Parameter] public EventCallback OnSubmit { get; set; }

        /// <summary>
        /// Shows "or" and "Choose another method".
        /// </summary>
        [Parameter] public EventCallback OnAlternative { get; set; }

        /// <summary>
        /// Shows "Already have an account? Sign in".
        /// </summary>
        [Parameter] public EventCallback OnSignIn { get; set; }

        /// <summary>
        /// Shows the "Back to sign in" text button.
        /// </summary>
        [Parameter] public EventCallback OnBackToLogin { get; set; }
    }
}
