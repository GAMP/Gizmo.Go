using System.Globalization;
using Gizmo.Go.Core.Models.Balance;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Home
{
    /// <summary>
    /// Main balance and bonuses cards. A failed or missing balance is shown as a dash. As the client: the balance in the
    /// currency format of the culture (the server currency options applied at start), the bonuses as a plain number.
    /// </summary>
    public partial class BalanceCards : ComponentBase
    {
        private const string Unavailable = "—";

        [Inject]
        private ILocalizationService LocalizationService { get; set; } = null!;

        [Parameter]
        public UserBalance? Balance { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        private string MainBalanceText => Balance is null ? Unavailable : Balance.Balance.ToString("C", CultureInfo.CurrentCulture);

        private string BonusesText => Balance is null ? Unavailable : Balance.Points.ToString(CultureInfo.CurrentCulture);
    }
}
