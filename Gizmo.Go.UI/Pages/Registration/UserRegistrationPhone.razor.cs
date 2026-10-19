using System.Linq.Expressions;
using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationPhone : ComponentBase, IDisposable
    {
        [Inject]
        private UserRegistrationPhoneViewState UserRegistrationPhoneViewState { get; set; } = null!;

        [Inject]
        private UserRegistrationPhoneViewService UserRegistrationPhoneViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationPhoneViewState);
            base.OnInitialized();
        }

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);

            return UserRegistrationPhoneViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private Task OnRegionCodeChanged(string regionCode)
        {
            var country = UserRegistrationPhoneViewState.Countries.FirstOrDefault(c => string.Equals(c.RegionCode, regionCode, StringComparison.OrdinalIgnoreCase));

            if (country is null)
            {
                UserRegistrationPhoneViewService.SetCountry(null);
                UserRegistrationPhoneViewService.SetRegionCode(null);
                UserRegistrationPhoneViewService.SetMobilePhone(null);
            }
            else
            {
                UserRegistrationPhoneViewService.SetCountry(country.CountryName);
                UserRegistrationPhoneViewService.SetRegionCode(country.RegionCode);
            }

            return Task.CompletedTask;
        }

        private Task OnPhoneValueChanged(string value)
        {
            UserRegistrationPhoneViewService.SetMobilePhone(value);
            return Task.CompletedTask;
        }

        private async Task SubmitAsync() => await UserRegistrationPhoneViewService.SubmitAsync();

        private async Task NavigateBack() => await UserRegistrationPhoneViewService.NavigateBackAsync();

        private async Task NavigateToLogin() => await UserRegistrationPhoneViewService.NavigateToLoginAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationPhoneViewState);
        }
    }
}
