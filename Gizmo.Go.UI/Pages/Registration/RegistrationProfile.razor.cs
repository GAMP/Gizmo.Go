using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationProfile : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        private RegistrationProfileViewState RegistrationProfileViewState { get; set; } = null!;

        [Inject]
        private RegistrationProfileViewService RegistrationProfileViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region FIELDS

        private string _usernameInputValue = string.Empty;
        private CancellationTokenSource? _usernameDebounceCts;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationProfileViewState);
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationProfileViewState);
            _usernameDebounceCts?.Cancel();
            _usernameDebounceCts?.Dispose();
        }

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);
            return RegistrationProfileViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private async Task OnUsernameInput(ChangeEventArgs e)
        {
            _usernameDebounceCts?.Cancel();
            _usernameDebounceCts = new CancellationTokenSource();

            _usernameInputValue = e.Value?.ToString() ?? string.Empty;

            try
            {
                await Task.Delay(300, _usernameDebounceCts.Token);
                await RegistrationProfileViewService.SetUsernameAsync(_usernameInputValue);
            }
            catch (OperationCanceledException) { }
        }

        private async Task OnFirstNameInput(ChangeEventArgs e) =>
            await RegistrationProfileViewService.SetFirstNameAsync(e.Value?.ToString() ?? string.Empty);

        private async Task OnLastNameInput(ChangeEventArgs e) =>
            await RegistrationProfileViewService.SetLastNameAsync(e.Value?.ToString() ?? string.Empty);

        private async Task OnBirthDateInput(ChangeEventArgs e)
        {
            var raw = e.Value?.ToString();
            DateTime? parsed = DateTime.TryParse(raw, out var dt) ? dt : null;
            await RegistrationProfileViewService.SetBirthDateAsync(parsed);
        }

        private async Task NavigateBack() =>
            await RegistrationProfileViewService.NavigateBackAsync();

        private async Task SubmitAsync() =>
            await RegistrationProfileViewService.SubmitAsync();

        #endregion
    }
}
