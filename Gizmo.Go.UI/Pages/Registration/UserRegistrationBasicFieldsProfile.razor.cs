using System.Linq.Expressions;
using Gizmo.Go.Core.Models;
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

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationBasicFieldsViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationBasicFieldsViewState);
        }

        // shown fields are required by the server (default user group required info) and are validated
        // while typing; the form IsValid is already true after the account step, so it cannot gate the button
        private bool IsSubmitDisabled
        {
            get
            {
                var state = UserRegistrationBasicFieldsViewState;

                return state.IsLoading
                    || state.IsValid == false
                    || (state.ShowFirstName && (string.IsNullOrWhiteSpace(state.FirstName) || HasErrors(() => state.FirstName)))
                    || (state.ShowLastName && (string.IsNullOrWhiteSpace(state.LastName) || HasErrors(() => state.LastName)))
                    || (state.ShowBirthDate && (!state.BirthDate.HasValue || HasErrors(() => state.BirthDate)))
                    || (state.ShowSex && (state.Sex == UserSex.Unspecified || HasErrors(() => state.Sex)))
                    || (state.ShowMobilePhone && (string.IsNullOrEmpty(state.MobilePhone) || HasErrors(() => state.MobilePhone)))
                    || (state.ShowEmail && (string.IsNullOrWhiteSpace(state.Email) || HasErrors(() => state.Email)))
                    || (state.ShowPhone && (string.IsNullOrWhiteSpace(state.Phone) || HasErrors(() => state.Phone)));
            }
        }

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);
            return UserRegistrationBasicFieldsViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private Task OnFirstNameChanged(string? value)
        {
            UserRegistrationBasicFieldsViewService.SetFirstName(value);
            return Task.CompletedTask;
        }

        private Task OnLastNameChanged(string? value)
        {
            UserRegistrationBasicFieldsViewService.SetLastName(value);
            return Task.CompletedTask;
        }

        private string? GetFirstError<T>(Expression<Func<T>> accessor) =>
            UserRegistrationBasicFieldsViewService.EditContext.GetValidationMessages(FieldIdentifier.Create(accessor)).FirstOrDefault();

        private Task OnBirthDateChanged(DateTime? value)
        {
            UserRegistrationBasicFieldsViewService.SetBirthDate(value);
            return Task.CompletedTask;
        }

        private Task OnSexChanged(UserSex value)
        {
            UserRegistrationBasicFieldsViewService.SetSex(value);
            return Task.CompletedTask;
        }

        private Task OnEmailChanged(string? value)
        {
            UserRegistrationBasicFieldsViewService.SetEmail(value);
            return Task.CompletedTask;
        }

        private Task OnPhoneChanged(string? value)
        {
            UserRegistrationBasicFieldsViewService.SetPhone(value);
            return Task.CompletedTask;
        }

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
