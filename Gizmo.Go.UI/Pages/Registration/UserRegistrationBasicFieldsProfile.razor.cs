using System.Linq.Expressions;
using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationBasicFieldsProfile : ComponentBase, IDisposable
    {
        [Inject]
        private UserRegistrationBasicFieldsViewState UserRegistrationBasicFieldsViewState { get; set; } = null!;

        [Inject]
        private UserRegistrationBasicFieldsViewService UserRegistrationBasicFieldsViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        private string _usernameInputValue = string.Empty;
        private CancellationTokenSource? _usernameDebounceCts;

        protected override void OnInitialized()
        {
            _usernameInputValue = UserRegistrationBasicFieldsViewState.Username;
            this.SubscribeChange(UserRegistrationBasicFieldsViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationBasicFieldsViewState);
            _usernameDebounceCts?.Cancel();
            _usernameDebounceCts?.Dispose();
        }

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);
            return UserRegistrationBasicFieldsViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private async Task OnUsernameInput(ChangeEventArgs e)
        {
            _usernameDebounceCts?.Cancel();
            _usernameDebounceCts = new CancellationTokenSource();

            _usernameInputValue = e.Value?.ToString() ?? string.Empty;

            try
            {
                await Task.Delay(300, _usernameDebounceCts.Token);
                UserRegistrationBasicFieldsViewService.SetUsername(_usernameInputValue);
            }
            catch (OperationCanceledException) { }
        }

        private void OnFirstNameInput(ChangeEventArgs e) =>
            UserRegistrationBasicFieldsViewService.SetFirstName(e.Value?.ToString());

        private void OnLastNameInput(ChangeEventArgs e) =>
            UserRegistrationBasicFieldsViewService.SetLastName(e.Value?.ToString());

        private void OnBirthDateInput(ChangeEventArgs e)
        {
            DateTime? parsed = DateTime.TryParse(e.Value?.ToString(), out var dt) ? dt : null;
            UserRegistrationBasicFieldsViewService.SetBirthDate(parsed);
        }

        private void OnSexChanged(ChangeEventArgs e)
        {
            var sex = Enum.TryParse<UserSex>(e.Value?.ToString(), out var parsed) ? parsed : UserSex.Unspecified;
            UserRegistrationBasicFieldsViewService.SetSex(sex);
        }

        private void OnEmailInput(ChangeEventArgs e) =>
            UserRegistrationBasicFieldsViewService.SetEmail(e.Value?.ToString());

        private void OnPhoneInput(ChangeEventArgs e) =>
            UserRegistrationBasicFieldsViewService.SetPhone(e.Value?.ToString());

        private Task OnPhoneRegionCodeChanged(string regionCode)
        {
            UserRegistrationBasicFieldsViewService.SetPhoneRegionCode(string.IsNullOrEmpty(regionCode) ? null : regionCode);
            return Task.CompletedTask;
        }

        private Task OnMobilePhoneChanged(string value)
        {
            UserRegistrationBasicFieldsViewService.SetMobilePhone(value);
            return Task.CompletedTask;
        }

        private async Task NavigateBack() =>
            await UserRegistrationBasicFieldsViewService.NavigateBackToAccountAsync();

        private async Task SubmitAsync() =>
            await UserRegistrationBasicFieldsViewService.SubmitAsync();
    }
}
