using System.Linq.Expressions;
using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationEmailAddPhone : ComponentBase, IDisposable
    {
        [Inject] private RegistrationEmailAddPhoneViewState RegistrationEmailAddPhoneViewState { get; set; } = null!;

        [Inject] private RegistrationEmailAddPhoneViewService RegistrationEmailAddPhoneViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationEmailAddPhoneViewState);
            base.OnInitialized();
        }

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);

            return RegistrationEmailAddPhoneViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        private async Task SubmitAsync() => await RegistrationEmailAddPhoneViewService.SubmitAsync();

        private async Task NavigateBack() => await RegistrationEmailAddPhoneViewService.NavigateBackAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationEmailAddPhoneViewState);
        }
    }
}
