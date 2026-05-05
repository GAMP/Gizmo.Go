using System.Threading;
using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationEmail : ComponentBase, IDisposable
    {
        [Inject] private RegistrationEmailViewState RegistrationEmailViewState { get; set; } = null!;

        [Inject] private RegistrationEmailViewService RegistrationEmailViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        private CancellationTokenSource? _emailDebounceCts;

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationEmailViewState);
            base.OnInitialized();
        }

        private async Task OnEmailInput(ChangeEventArgs e)
        {
            _emailDebounceCts?.Cancel();
            _emailDebounceCts = new CancellationTokenSource();

            var value = e.Value?.ToString() ?? string.Empty;

            try
            {
                await Task.Delay(300, _emailDebounceCts.Token);
                await RegistrationEmailViewService.SetEmailAsync(value);
            }
            catch (OperationCanceledException) { }
        }

        private async Task SubmitAsync() =>
            await RegistrationEmailViewService.SubmitAsync();

        private async Task NavigateBack() =>
            await RegistrationEmailViewService.NavigateBackAsync();

        private async Task NavigateToAlternative() =>
            await RegistrationEmailViewService.NavigateToAlternativeAsync();

        private async Task NavigateToLogin() =>
            await RegistrationEmailViewService.NavigateToLoginAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationEmailViewState);
            _emailDebounceCts?.Cancel();
            _emailDebounceCts?.Dispose();
        }
    }
}
