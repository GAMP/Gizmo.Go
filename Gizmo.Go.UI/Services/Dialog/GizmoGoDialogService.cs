using Gizmo.Client.UI;
using Gizmo.Go.UI.Components.Dialog;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Go.UI.Services.Dialog
{
    public sealed class GizmoGoDialogService : DialogServiceBase, IGizmoGoDialogService
    {
        public GizmoGoDialogService(
            IOptionsMonitor<DialogOptions> options,
            IServiceProvider serviceProvider,
            ILogger<GizmoGoDialogService> logger) : base(options, serviceProvider, logger)
        {
        }

        public Task<AddDialogResult<UserAgreementResult>> ShowUserAgreementDialogAsync(UserAgreementDialogParameters userAgreementDialogParameters, CancellationToken cancellationToken = default)
        {
            var parameters = userAgreementDialogParameters.ToDictionary();

            return ShowDialogAsync<UserAgreementDialog, UserAgreementResult>(parameters, new DialogDisplayOptions()
            {
                Closable = false,
                CloseOnClick = false
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<UserAgreementResult>> ShowReadOnlyUserAgreementDialogAsync(UserAgreementDialogParameters userAgreementDialogParameters, CancellationToken cancellationToken = default)
        {
            var parameters = userAgreementDialogParameters.ToDictionary();
            parameters["IsReadOnly"] = true;

            return ShowDialogAsync<UserAgreementDialog, UserAgreementResult>(parameters, new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = true
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<EmptyComponentResult>> ShowConfirmDialogAsync(string title, string text, string confirmText, bool isDanger = false, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, object>
            {
                [nameof(ConfirmDialog.Title)] = title,
                [nameof(ConfirmDialog.Text)] = text,
                [nameof(ConfirmDialog.ConfirmText)] = confirmText,
                [nameof(ConfirmDialog.IsDanger)] = isDanger
            };

            return ShowDialogAsync<ConfirmDialog, EmptyComponentResult>(parameters, new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = true
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<EmptyComponentResult>> ShowLadderInfoDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<LadderInfoDialog, EmptyComponentResult>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = true
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<EmptyComponentResult>> ShowRulesDialogAsync(string title, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, object>
            {
                [nameof(RulesDialog.Title)] = title
            };

            return ShowDialogAsync<RulesDialog, EmptyComponentResult>(parameters, new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = true
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<BookingNoOptionsResult>> ShowBookingNoOptionsDialogAsync(CancellationToken cancellationToken = default)
        {
            return ShowDialogAsync<BookingNoOptionsDialog, BookingNoOptionsResult>(new Dictionary<string, object>(), new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = true
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<BookingHostTakenResult>> ShowBookingHostTakenDialogAsync(IReadOnlyList<BookingHostTakenItem> hosts, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, object>
            {
                [nameof(BookingHostTakenDialog.Hosts)] = hosts
            };

            // an overlay click dismisses the sheet: the caller treats it as "back to the selection"
            return ShowDialogAsync<BookingHostTakenDialog, BookingHostTakenResult>(parameters, new DialogDisplayOptions()
            {
                Closable = true,
                CloseOnClick = true
            }, default, cancellationToken);
        }

        public Task<AddDialogResult<BookingSuccessResult>> ShowBookingSuccessDialogAsync(string hosts, string period, string cost, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, object>
            {
                [nameof(BookingSuccessDialog.Hosts)] = hosts,
                [nameof(BookingSuccessDialog.Period)] = period,
                [nameof(BookingSuccessDialog.Cost)] = cost
            };

            return ShowDialogAsync<BookingSuccessDialog, BookingSuccessResult>(parameters, new DialogDisplayOptions()
            {
                Closable = false,
                CloseOnClick = false
            }, default, cancellationToken);
        }
    }
}
