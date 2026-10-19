using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class UserRegistrationEmail : ComponentBase, IDisposable
    {
        [Inject] private UserRegistrationEmailViewState UserRegistrationEmailViewState { get; set; } = null!;

        [Inject] private UserRegistrationEmailViewService UserRegistrationEmailViewService { get; set; } = null!;

        [Inject] private ILocalizationService LocalizationService { get; set; } = null!;

        private CancellationTokenSource? _emailDebounceCts;

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationEmailViewState);
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
                UserRegistrationEmailViewService.SetEmail(value);
            }
            catch (OperationCanceledException) { }
        }

        private async Task SubmitAsync() =>
            await UserRegistrationEmailViewService.SubmitAsync();

        private async Task NavigateBack() =>
            await UserRegistrationEmailViewService.NavigateBackAsync();

        private async Task NavigateToAlternative() =>
            await UserRegistrationEmailViewService.NavigateToAlternativeAsync();

        private async Task NavigateToLogin() =>
            await UserRegistrationEmailViewService.NavigateToLoginAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationEmailViewState);
            _emailDebounceCts?.Cancel();
            _emailDebounceCts?.Dispose();
        }
    }
}
