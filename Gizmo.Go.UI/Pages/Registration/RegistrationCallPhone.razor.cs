using Gizmo.Go.UI.View.Services.Pages.Registration;
using Gizmo.Go.UI.View.States.Pages.Registration;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Pages.Registration
{
    public partial class RegistrationCallPhone : ComponentBase, IDisposable
    {
        #region PROPERTIES

        [Inject]
        private RegistrationCallPhoneViewState RegistrationCallPhoneViewState { get; set; } = null!;

        [Inject]
        private RegistrationCallPhoneViewService RegistrationCallPhoneViewService { get; set; } = null!;

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        #endregion

        #region FIELDS

        private string _phoneInputValue = string.Empty;

        private CancellationTokenSource? _phoneInputDebounceCts;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(RegistrationCallPhoneViewState);
            base.OnInitialized();
        }

        #endregion

        #region METHODS

        private async Task OnPhoneInputAsync(ChangeEventArgs args)
        {
            _phoneInputDebounceCts?.Cancel();
            _phoneInputDebounceCts = new CancellationTokenSource();

            _phoneInputValue = args.Value?.ToString() ?? string.Empty;

            try
            {
                await Task.Delay(150, _phoneInputDebounceCts.Token);
                await RegistrationCallPhoneViewService.UpdatePhoneAsync(_phoneInputValue, RegistrationCallPhoneViewState.SelectedCountryIso2);
            }
            catch (TaskCanceledException)
            {
                // ввод продолжается — игнорируем
            }
        }

        private async Task OnPhoneFormattedAsync(ChangeEventArgs args)
        {
            _phoneInputValue = args.Value?.ToString() ?? string.Empty;
            await RegistrationCallPhoneViewService.UpdatePhoneAsync(_phoneInputValue, RegistrationCallPhoneViewState.SelectedCountryIso2);

            if (!string.IsNullOrEmpty(RegistrationCallPhoneViewState.FormattedPhoneInput))
                _phoneInputValue = RegistrationCallPhoneViewState.FormattedPhoneInput;
        }

        private async Task SubmitAsync() => await RegistrationCallPhoneViewService.SubmitAsync();

        private async Task NavigateBack() => await RegistrationCallPhoneViewService.NavigateBackAsync();

        public void Dispose()
        {
            this.UnsubscribeChange(RegistrationCallPhoneViewState);
            _phoneInputDebounceCts?.Cancel();
            _phoneInputDebounceCts?.Dispose();
        }

        #endregion
    }
}
