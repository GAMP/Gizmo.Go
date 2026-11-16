using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Go.UI.Helpers;

public static class NavigationHelper
{
    public const string WelcomePage = "/";
    public const string LoginPage = "/login";
    public const string HomePage = "/home";
    public const string ProfileRoute = "/profile";
    public const string NewsRoute = "/news";
    public const string NewsDetailsRoute = "/news/{id:int}";
    public const string ReservationsRoute = "/reservations";
    public const string ClubRoute = "/clubs/{id:int}";
    public const string BookingRoute = "/clubs/{id:int}/booking";
    public const string ProfileLanguageRoute = "/profile/language";
    public const string ProfilePoliciesRoute = "/profile/policies";
    public const string ProfilePolicyRoute = "/profile/policies/{id:int}";
    public const string ProfileAboutRoute = "/profile/about";
    public const string ProfileAboutDocumentRoute = "/profile/about/{id:int}";
    public const string ProfileChangePasswordRoute = "/profile/password";

    /// <summary>History entry state of a navigation made inside the app (a back step stays in the app).</summary>
    public const string InAppHistoryState = "app";

    /// <summary>
    /// Options of a forward navigation inside the main zone: the new history entry is marked with <see cref="InAppHistoryState"/>,
    /// so its back button returns through the history (the browser, the PWA and the Android back button stay in step).
    /// </summary>
    public static NavigationOptions InAppNavigation => new() { HistoryEntryState = InAppHistoryState };

    /// <summary>Gets whether the current history entry was opened by an in-app navigation (<see cref="InAppNavigation"/>).</summary>
    public static bool IsInAppEntry(string? historyEntryState) => historyEntryState == InAppHistoryState;

    public static string GetNewsDetailsRoute(int id) => $"{NewsRoute}/{id}";

    public static string GetClubRoute(int id) => $"/clubs/{id}";

    /// <summary>Gets the booking route of the club at the step (1 session, 2 PC selection, 3 confirmation).</summary>
    public static string GetBookingRoute(int id, int step = 1) => $"/clubs/{id}/booking?step={step}";

    /// <summary>Gets the reservations route that shows only the reservation with the specified id.</summary>
    public static string GetReservationRoute(int id) => $"{ReservationsRoute}?id={id}";

    /// <summary>Gets the reservations route that shows only the nearest reservation (the Home block) with the specified id.</summary>
    public static string GetNearestReservationRoute(int id) => $"{ReservationsRoute}?id={id}&nearest=true";

    public static string GetProfilePolicyRoute(int id) => $"{ProfilePoliciesRoute}/{id}";

    public static string GetProfileAboutDocumentRoute(int id) => $"{ProfileAboutRoute}/{id}";

    /// <summary>
    /// Back button of a main zone screen: an entry opened inside the app steps back through the history; an entry opened
    /// directly (a link, a reload without the state) has nothing in the app to return to and opens <paramref name="fallbackRoute"/>.
    /// </summary>
    public static async Task GoBackOrNavigateAsync(this NavigationService navigationService, string? historyEntryState, string fallbackRoute)
    {
        if (IsInAppEntry(historyEntryState))
            await navigationService.GoBackAsync();
        else
            navigationService.NavigateTo(fallbackRoute, new NavigationOptions { ReplaceHistoryEntry = true });
    }

    /// <summary>
    /// Leaves a flow of several history entries (the booking steps): steps back over the <paramref name="flowEntries"/>
    /// entries of the flow to the screen it was opened from, so a back step never returns into the flow. The browser cannot
    /// delete history entries; the flow entries become forward entries and are dropped by the next navigation.
    /// </summary>
    /// <returns>True when the history was stepped back; false when nothing was done (no entries, no JS runtime).</returns>
    public static async Task<bool> LeaveFlowAsync(this NavigationService navigationService, JSRuntimeService jsRuntimeService, int flowEntries)
    {
        if (flowEntries <= 0 || jsRuntimeService.JSRuntime is not { } jsRuntime)
            return false;

        var locationChanged = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e) => locationChanged.TrySetResult();

        navigationService.LocationChanged += OnLocationChanged;
        try
        {
            await jsRuntime.InvokeVoidAsync("window.history.go", -flowEntries);

            // the next navigation must apply to the entry the history stepped back to
            await Task.WhenAny(locationChanged.Task, Task.Delay(TimeSpan.FromSeconds(2)));
        }
        finally
        {
            navigationService.LocationChanged -= OnLocationChanged;
        }

        return true;
    }

    public const string ProfileCompletionRoute = "/profile-completion";

    public const string RegistrationIndexRoute = "/registration-index";
    public const string RegistrationProvidersRoute = "/registration-providers";
    public const string RegistrationPhoneRoute = "/registration-phone";
    public const string RegistrationEmailRoute = "/registration-email";
    public const string RegistrationConfirmationRoute = "/registration-confirmation";
    public const string RegistrationRedirectRoute = "/registration-redirect";
    public const string RegistrationRedirectSuccessRoute = "/registration-redirect-success";
    public const string RegistrationBasicFieldsRoute = "/registration-basic-fields";
    public const string RegistrationBasicFieldsProfileRoute = "/registration-basic-fields-profile";
    public const string RegistrationAdditionalFieldsRoute = "/registration-additional-fields";
    public const string RegistrationErrorRoute = "/registration-error";
    public const string RegistrationSuccessRoute = "/registration-success";
    public const string RegistrationCallPhoneRoute = "/registration-call-phone";
    public const string RegistrationCallVerifyRoute = "/registration-call-verify";
    public const string RegistrationCallSuccessRoute = "/registration-call-success";

    public const string PasswordRecoveryRoute = "/password-recovery";
    public const string PasswordRecoveryKindRoute = "/password-recovery-kind";
    public const string PasswordRecoveryDestinationRoute = "/password-recovery-destination";
    public const string PasswordRecoveryAccountRoute = "/password-recovery-account";
    public const string PasswordRecoveryConfirmationRoute = "/password-recovery-confirmation";
    public const string PasswordRecoverySetNewPasswordRoute = "/password-recovery-set-new-password";
    public const string PasswordRecoverySuccessRoute = "/password-recovery-success";
}
