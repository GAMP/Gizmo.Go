using Gizmo.Client.UI;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Dialog
{
    public partial class UserAgreementDialog : ComponentBase
    {
        private bool _accepted;
        private string? _shownName;
        private string? _shownAgreement;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter]
        public string Name { get; set; } = string.Empty;

        [Parameter]
        public string Agreement { get; set; } = string.Empty;

        [Parameter]
        public bool IsRejectable { get; set; }

        /// <summary>
        /// Shows the agreement for reading only: no accept checkbox, a single close button.
        /// </summary>
        [Parameter]
        public bool IsReadOnly { get; set; }

        [Parameter]
        public DialogDisplayOptions? DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<UserAgreementResult> ResultCallback { get; set; }

        // ErrorCallback, SuspendTimeoutCallback and Identifier are always supplied by the dialog controller (IComponentController.Parameters)
        [Parameter]
        public EventCallback<Exception> ErrorCallback { get; set; }

        [Parameter]
        public EventCallback<bool> SuspendTimeoutCallback { get; set; }

        [Parameter]
        public int Identifier { get; set; }

        private void OnAcceptedChanged(ChangeEventArgs e)
        {
            _accepted = e.Value is true;
        }

        private async Task ContinueAsync()
        {
            await ResultCallback.InvokeAsync(new UserAgreementResult() { Accepted = _accepted });
        }

        private async Task CloseAsync()
        {
            // in the registration flow closing dismisses the agreement; the caller routes a mandatory one back to login
            if (IsReadOnly)
                await ContinueAsync();
            else
                await DismissCallback.InvokeAsync();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // the host re-renders on clicks inside the dialog; reset the choice only for a new agreement
            if (_shownName == Name && _shownAgreement == Agreement)
                return;

            _shownName = Name;
            _shownAgreement = Agreement;
            _accepted = false;
        }
    }
}
