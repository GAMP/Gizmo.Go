using System.Linq.Expressions;
using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationPhone : ComponentBase, IDisposable
    {
        [Inject]
        private RegistrationPhoneViewState RegistrationPhoneViewState { get; set; } = null!;

        [Inject]
        private RegistrationPhoneViewService RegistrationPhoneViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationPhoneViewState);
            base.OnInitialized();
        }

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);

            return RegistrationPhoneViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private async Task ToggleTerms() => await RegistrationPhoneViewService.ToggleTermsAsync(); //  что за Terms откуда и для чего нужен?

        private void OnTermsKeyDown(KeyboardEventArgs args)
        {
            if (args.Key is " " or "Enter")
                _ = RegistrationPhoneViewService.ToggleTermsAsync();
        }

        private async Task SubmitAsync() => await RegistrationPhoneViewService.SubmitAsync();

        private async Task NavigateToLogin() => await RegistrationPhoneViewService.NavigateBackAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationPhoneViewState);
        }
    }
}
