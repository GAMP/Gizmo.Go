using System.Linq.Expressions;
using Gizmo.Go.Core.Models;
using Gizmo.Go.UI.View.Services.Pages;
using Gizmo.Go.UI.View.States.Pages;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Gizmo.Go.UI.Pages
{
    public partial class ProfileCompletion : ComponentBase, IDisposable
    {
        [Inject]
        private ProfileCompletionViewState ProfileCompletionViewState { get; set; } = null!;

        [Inject]
        private ProfileCompletionViewService ProfileCompletionViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(ProfileCompletionViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(ProfileCompletionViewState);
        }

        // shown fields are required by the server and validated while typing; the form IsValid stays null
        // until the first validation, so it cannot gate the button on its own
        private bool IsSubmitDisabled
        {
            get
            {
                var state = ProfileCompletionViewState;

                return state.IsLoading
                    || state.IsValid == false
                    || (state.ShowFirstName && (string.IsNullOrWhiteSpace(state.FirstName) || HasErrors(() => state.FirstName)))
                    || (state.ShowLastName && (string.IsNullOrWhiteSpace(state.LastName) || HasErrors(() => state.LastName)))
                    || (state.ShowBirthDate && (!state.BirthDate.HasValue || HasErrors(() => state.BirthDate)))
                    || (state.ShowSex && (state.Sex == UserSex.Unspecified || HasErrors(() => state.Sex)))
                    || (state.ShowMobilePhone && (string.IsNullOrEmpty(state.MobilePhone) || HasErrors(() => state.MobilePhone)))
                    || (state.ShowEmail && (string.IsNullOrWhiteSpace(state.Email) || HasErrors(() => state.Email)))
                    || (state.ShowPhone && (string.IsNullOrWhiteSpace(state.Phone) || HasErrors(() => state.Phone)))
                    || (state.ShowCountry && (string.IsNullOrWhiteSpace(state.Country) || HasErrors(() => state.Country)))
                    || (state.ShowAddress && (string.IsNullOrWhiteSpace(state.Address) || HasErrors(() => state.Address)))
                    || (state.ShowCity && (string.IsNullOrWhiteSpace(state.City) || HasErrors(() => state.City)))
                    || (state.ShowPostCode && (string.IsNullOrWhiteSpace(state.PostCode) || HasErrors(() => state.PostCode)));
            }
        }

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);
            return ProfileCompletionViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private string? GetFirstError<T>(Expression<Func<T>> accessor) =>
            ProfileCompletionViewService.EditContext.GetValidationMessages(FieldIdentifier.Create(accessor)).FirstOrDefault();

        private Task OnFirstNameChanged(string? value)
        {
            ProfileCompletionViewService.SetFirstName(value);
            return Task.CompletedTask;
        }

        private Task OnLastNameChanged(string? value)
        {
            ProfileCompletionViewService.SetLastName(value);
            return Task.CompletedTask;
        }

        private Task OnBirthDateChanged(DateTime? value)
        {
            ProfileCompletionViewService.SetBirthDate(value);
            return Task.CompletedTask;
        }

        private Task OnSexChanged(UserSex value)
        {
            ProfileCompletionViewService.SetSex(value);
            return Task.CompletedTask;
        }

        private Task OnPhoneRegionCodeChanged(string regionCode)
        {
            ProfileCompletionViewService.SetPhoneRegionCode(string.IsNullOrEmpty(regionCode) ? null : regionCode);
            return Task.CompletedTask;
        }

        private Task OnMobilePhoneChanged(string value)
        {
            ProfileCompletionViewService.SetMobilePhone(value);
            return Task.CompletedTask;
        }

        private Task OnEmailChanged(string? value)
        {
            ProfileCompletionViewService.SetEmail(value);
            return Task.CompletedTask;
        }

        private Task OnPhoneChanged(string? value)
        {
            ProfileCompletionViewService.SetPhone(value);
            return Task.CompletedTask;
        }

        private Task OnCountryChanged(string? value)
        {
            ProfileCompletionViewService.SetCountry(value);
            return Task.CompletedTask;
        }

        private Task OnAddressChanged(string? value)
        {
            ProfileCompletionViewService.SetAddress(value);
            return Task.CompletedTask;
        }

        private Task OnCityChanged(string? value)
        {
            ProfileCompletionViewService.SetCity(value);
            return Task.CompletedTask;
        }

        private Task OnPostCodeChanged(string? value)
        {
            ProfileCompletionViewService.SetPostCode(value);
            return Task.CompletedTask;
        }

        private async Task SubmitAsync() => await ProfileCompletionViewService.SubmitAsync();
    }
}
