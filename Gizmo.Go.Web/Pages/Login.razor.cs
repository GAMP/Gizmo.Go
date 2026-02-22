using System.Linq.Expressions;
using Gizmo.Go.Web.View.Services;
using Gizmo.Go.Web.View.States;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Go.Web.Pages
{
    public partial class Login : ComponentBase, IDisposable
    {
        [Inject]
        private LoginViewState LoginViewState { get; set; } = null!;

        [Inject]
        private LoginViewService LoginViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        protected override void OnInitialized()
        {
            this.SubscribeChange(LoginViewState);
            base.OnInitialized();
        }

        private async Task OnKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
                await SubmitAsync();
        }

        private async Task SubmitAsync()
        {
            await LoginViewService.SubmitAsync();
        }

        private bool HasErrors<T>(Expression<Func<T>> accessor)
        {
            var fieldIdentifier = FieldIdentifier.Create(accessor);
            return LoginViewService.EditContext.GetValidationMessages(fieldIdentifier).Any();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(LoginViewState);
        }
    }
}
