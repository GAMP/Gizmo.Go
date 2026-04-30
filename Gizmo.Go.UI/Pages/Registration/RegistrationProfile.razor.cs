using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Gizmo.Go.Core.Models.Registration;
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

        private async Task OnEmailInput(ChangeEventArgs e) =>
            await RegistrationProfileViewService.SetEmailAsync(e.Value?.ToString() ?? string.Empty);

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

        private async Task OnAddressInput(ChangeEventArgs e) =>
            await RegistrationProfileViewService.SetAddressAsync(e.Value?.ToString() ?? string.Empty);

        private async Task OnCityInput(ChangeEventArgs e) =>
            await RegistrationProfileViewService.SetCityAsync(e.Value?.ToString() ?? string.Empty);

        private async Task OnCountryInput(ChangeEventArgs e) =>
            await RegistrationProfileViewService.SetCountryAsync(e.Value?.ToString() ?? string.Empty);

        private async Task OnPostCodeInput(ChangeEventArgs e) =>
            await RegistrationProfileViewService.SetPostCodeAsync(e.Value?.ToString() ?? string.Empty);

        private async Task OnPhoneInput(ChangeEventArgs e) =>
            await RegistrationProfileViewService.SetPhoneAsync(e.Value?.ToString() ?? string.Empty);

        private async Task OnMobilePhoneInput(ChangeEventArgs e) =>
            await RegistrationProfileViewService.SetMobilePhoneAsync(e.Value?.ToString() ?? string.Empty);

        private async Task OnSexChange(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var val) && Enum.IsDefined(typeof(UserSex), val))
                await RegistrationProfileViewService.SetSexAsync((UserSex)val);
        }

        private async Task NavigateBack() =>
            await RegistrationProfileViewService.NavigateBackAsync();

        private async Task SubmitAsync() =>
            await RegistrationProfileViewService.SubmitAsync();

        #endregion
    }
}
