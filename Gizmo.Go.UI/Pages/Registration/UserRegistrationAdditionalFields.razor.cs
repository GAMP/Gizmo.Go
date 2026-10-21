using System.Linq.Expressions;
using Gizmo.Go.UI.Services.Registration;
using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationAdditionalFields : ComponentBase, IDisposable
    {
        private const int PostCodeMaxLength = 20;

        [Inject]
        private UserRegistrationAdditionalFieldsViewState UserRegistrationAdditionalFieldsViewState { get; set; } = null!;

        [Inject]
        private UserRegistrationAdditionalFieldsViewService UserRegistrationAdditionalFieldsViewService { get; set; } = null!;

        [Inject]
        private IRegistrationSessionService RegistrationSession { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        private bool ShowCountry => RegistrationSession.RequiredUserInfo?.Country == true;
        private bool ShowAddress => RegistrationSession.RequiredUserInfo?.Address == true;
        private bool ShowCity => RegistrationSession.RequiredUserInfo?.City == true;
        private bool ShowPostCode => RegistrationSession.RequiredUserInfo?.PostCode == true;

        // shown fields are required by the server and validated while typing; the form IsValid stays null
        // until the first validation, so it cannot gate the button on its own
        private bool IsSubmitDisabled
        {
            get
            {
                var state = UserRegistrationAdditionalFieldsViewState;

                return state.IsLoading
                    || state.IsValid == false
                    || (ShowCountry && (string.IsNullOrWhiteSpace(state.Country) || HasErrors(() => state.Country)))
                    || (ShowAddress && (string.IsNullOrWhiteSpace(state.Address) || HasErrors(() => state.Address)))
                    || (ShowCity && (string.IsNullOrWhiteSpace(state.City) || HasErrors(() => state.City)))
                    || (ShowPostCode && (string.IsNullOrWhiteSpace(state.PostCode) || HasErrors(() => state.PostCode)));
            }
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationAdditionalFieldsViewState);
            base.OnInitialized();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationAdditionalFieldsViewState);
        }

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);
            return UserRegistrationAdditionalFieldsViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private void OnCountryInput(ChangeEventArgs e) =>
            UserRegistrationAdditionalFieldsViewService.SetCountry(e.Value?.ToString());

        private void OnAddressInput(ChangeEventArgs e) =>
            UserRegistrationAdditionalFieldsViewService.SetAddress(e.Value?.ToString());

        private void OnCityInput(ChangeEventArgs e) =>
            UserRegistrationAdditionalFieldsViewService.SetCity(e.Value?.ToString());

        private void OnPostCodeInput(ChangeEventArgs e) =>
            UserRegistrationAdditionalFieldsViewService.SetPostCode(e.Value?.ToString());

        private async Task SubmitAsync() => await UserRegistrationAdditionalFieldsViewService.SubmitAsync();

        private async Task NavigateBack() => await UserRegistrationAdditionalFieldsViewService.NavigateBackAsync();
    }
}
