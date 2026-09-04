# Gizmo.Go code review — 2026-09-04

Hand review of branch `add-auth` (HEAD `4cfa489`), no build run, no code changed.
Cross-checked against `D:\Test\Gizmo.Server` (`version-3`) and the Manager UI's
`Gizmo.Web.Api.Client` submodule (`dev`). Status column is for tracking; update it
as findings are fixed.

## Scope

- `Gizmo.Go.Core`, `Gizmo.Go.Provider.Direct`, `Gizmo.Go.Provider.Platform`,
  `Gizmo.Go.UI`, `Gizmo.Go.Maui`, `Gizmo.Go.Web` — every `.cs`, `.razor`, host
  config, manifest and the JS lifecycle bridge.
- Submodule pins: `libs/Gizmo.Shared@version-3`,
  `libs/Gizmo.Web.Api.Client@telegram-verification-status`,
  `libs/Gizmo.Web.Api.Models@mrdpod`.

## Verified facts the findings rest on

| Fact | Evidence |
|---|---|
| Api.Models `mrdpod` is 152 commits behind `origin/version-3`, 0 ahead | `git rev-list --left-right --count origin/version-3...HEAD` in `libs/Gizmo.Web.Api.Models` |
| Api.Client `telegram-verification-status` is 59 behind `origin/dev`, 0 ahead | same command in `libs/Gizmo.Web.Api.Client` |
| Server registration controller is `UnsecuredUserApiControllerBase` → route `api/user/v3/registrations` | `Gizmo.Server/Web/Controllers/API/User/v3/RegistrationsController.cs`, base at `.../Base/User/UnsecuredUserApiControllerBase.cs:10` |
| Server actions: `GET methods` → `IReadOnlyList<AvailableVerificationMethodModel>`; `POST start` takes `UserRegistrationMethodStartModel {MethodId, Value}`; `POST confirmed`, `POST complete`, `POST direct` | same controller |
| Go client: `[WebApiRoute("api/v3/registrations")]`, `GET providers` → `VerificationProviderModel`, `POST start` sends `RegistrationStartModel {IntegrationPublicId, DeliveryMethod, PhoneNumber, Email}` | `libs/Gizmo.Web.Api.Client/.../Clients/v3/RegistrationsWebApiClient.cs` |
| Manager UI's Api.Client on `dev` already has `RegistrationsWebApiClient` at `api/user/v3/registrations` with `methods` | `Gizmo.Web.Manager.UI/Submodules/Gizmo.Web.Api.Client/.../RegistrationsWebApiClient.cs:11,20` |
| `api/v3/tokens/confirm` is `[AllowAnonymous]` | `Gizmo.Server/.../API/v3/TokensController.cs:41-44` |
| `api/v3/users/username/exist?username=` returns `bool`, controller is `SecuredAPIControllerWithService` (operator auth), no `[AllowAnonymous]` | `Gizmo.Server/.../API/v3/UsersController.cs:397-404` |
| `AddSecureWebApiClients` adds no auth handler; only `WithCurrentUICultureMessageHandler` is chained | `libs/Gizmo.Web.Api.Client/.../Infrastructure/Extensions/WebApiClientExtensions.cs`, `WebApiClientBuilder.cs` |
| `RegistrationsWebApiClient`, `TokensWebApiClient`, `UsersWebApiClient` are secure clients (no `[UnsecureWebApiClient]`); only the two `AuthWebApiClient`s and `PublicOptionsWebApiClient` are unsecure | grep of `Clients/v3` |
| `ViewServiceBase.InitializeAsync` is guarded by `IsInitialized` | `Gizmo.UI/View/Services/ViewServiceBase.cs:70-94` |
| Every `GO_*` key referenced in code exists in the UI resx; 23 keys are unused; no locale resx files exist; base resx is Russian | script over `Gizmo.Go.UI/Resources/Resources.resx` |
| `wwwroot/css/app.css` and `wwwroot/js/app.js` are not tracked | `git ls-files Gizmo.Go.UI/wwwroot` is empty |

## A. Blocking — the flow cannot work as pinned

| ID | Finding | Where | Fix | Status |
|---|---|---|---|---|
| A1 | **Registration API contract drift.** Every registration call targets `api/v3/registrations/...` with the old provider/request shapes; the server serves `api/user/v3/registrations` with `methods` + `MethodId`. All calls 404. | `Provider.Direct/Services/DirectRegistrationService.cs`, `Provider.Direct/Mappers/Registration/*`, `Core/Models/Registration/RegistrationProvider.cs`, `RegistrationStartRequest.cs` | Bump `libs/Gizmo.Web.Api.Models` and `libs/Gizmo.Web.Api.Client` to mainline (`version-3` / `dev`). Re-map Core models: provider list → `{Name, ChannelGuid, MethodId, CapabilityGuid, IsPrimary, HasChannel}`; start request → `{MethodId, Value}`. `RegistrationDeliveryMethod` and `IntegrationPublicId` disappear from the wire; redirect vs code-dispatch must be derived from `CapabilityGuid`. `RegistrationProvidersViewService` keys providers by `ChannelGuid` and passes `PublicId` through the query string — switch both to `MethodId`. | open |
| A2 | **Bearer token never attached.** Secure clients are registered without `.WithMessageHandler<BearerTokenHandler>()`. `BearerTokenHandler` and `CultureDelegatingHandler` are in DI in both hosts but on no pipeline. | `Provider.Direct/Extensions/ServiceCollectionExtensions.cs:42-44`; `Maui/MauiProgram.cs:79-80`; `Web/Program.cs:51-52` | Chain `.WithMessageHandler<BearerTokenHandler>()` on the secure builder (handler must be registered before `AddGizmoGoDirect` runs, which it is). Delete `CultureDelegatingHandler` — the client lib's `CurrentUICultureDelegatingHandler` already does this. | open |
| A3 | **Username-exists check is broken three ways.** Client path `users/username/{name}/exist` expecting `ExistResult`; server path `users/username/exist?username=` returning `bool`; server endpoint requires operator auth. Result: always throws → fail-open → duplicates only surface at `Complete`, which clears the session and bounces to providers with a generic error. | `Provider.Direct/Services/DirectUserService.cs:18-20`; `UI/View/Services/Pages/Registration/RegistrationProfileViewService.cs:136-145` | Needs a server-side anonymous user-API endpoint (or `[AllowAnonymous]` on the existing one, which leaks enumeration — decide deliberately) plus a client method matching it. Until then, on `CompleteAsync` returning `InvalidInput`/username-taken, stay on the profile page with the field error instead of clearing the session. | open |
| A4 | **Mobile hosts cannot reach an `http://` backend.** MAUI appsettings bakes `http://192.168.0.2`; `AndroidManifest.xml` has no `android:usesCleartextTraffic`, `Info.plist` has no `NSAppTransportSecurity`. Android 9+ and iOS refuse the connection. | `Maui/wwwroot/appsettings.json`; `Maui/Platforms/Android/AndroidManifest.xml`; `Maui/Platforms/iOS/Info.plist` | Ship https only; for dev builds add a `#if DEBUG`-only network security config rather than a manifest-wide cleartext flag. Backend URL should not be an embedded constant (see C1). | open |

## B. Correctness

| ID | Finding | Where | Fix | Status |
|---|---|---|---|---|
| B1 | **Any refresh failure logs out and deletes stored tokens.** `catch { await LogoutAsync() }` swallows `OperationCanceledException` and `HttpRequestException`. A request cancelled mid-refresh (navigation, component dispose) or a dropped connection ends the session permanently. | `Provider.Direct/Services/DirectAuthService.cs:174-195` | Rethrow cancellation; on transient errors keep the tokens and return the stale access token (or null) without touching storage; log out only on 401 / rejected refresh token. | open |
| B2 | **Refresh happens at the exact expiry instant, no 401 retry.** `_expiresUtc <= UtcNow` with no skew margin. | `DirectAuthService.cs:109,146` | Refresh when `ExpiresUtc - margin <= UtcNow` (30–60 s), and/or handle a 401 with `token-expired` header once via the client builder's `WithRetryPolicyHandler`. | open |
| B3 | **`LogoutAsync` mutates token fields outside `_tokenLock`.** Called both from UI and from inside `RefreshTokenAsync` (under lock). | `DirectAuthService.cs:88-98` | Split into a locked `ClearCoreAsync` and a public wrapper, or take the lock in `LogoutAsync` and have `RefreshTokenAsync` call the unlocked core. | open |
| B4 | **Auth state provider does network I/O and re-enters the lock.** `GetAuthenticationStateAsync` → `GetTokenAsync` may refresh; a failed refresh fires `StateChanged` → `NotifyAuthenticationStateChanged(GetAuthenticationStateAsync())` → `GetTokenAsync` again while the outer call still holds the semaphore. Resolves after release, but is fragile and repeats on every `AuthorizeView`. | `UI/Providers/GoAuthenticationStateProvider.cs:21-37` | Build claims from `IAuthService.CurrentUser`/cached token; never call the refreshing getter from the provider. | open |
| B5 | **Web host initialises twice.** `Program.cs` and `App.razor.cs` both call `TryRestoreSessionAsync` + `InitializeViewsServices`. View-service init is guarded, but session restore re-reads storage and can trigger a second refresh. | `Web/Program.cs:86-89`; `UI/App.razor.cs:39-40` | Remove the calls from `Program.cs` (the `App.razor.cs` path is needed for Hybrid and works for WASM too), or keep them in `Program.cs` and gate `App.razor.cs` on `AuthState != Unknown`. | open |
| B6 | **`MaskPhone` assumes a one-digit country code.** `+380501234567` → `+3 805 ***-**-67`. | `UI/Services/Registration/PhoneValidationService.cs:45-59` | Parse with libphonenumber, mask the national significant number, keep `+{countryCode}` intact. | open |
| B7 | **Post-login lands on Welcome.** Successful login with no `returnUrl` navigates to `""` (Welcome, which shows Sign In again). `RegistrationSuccessViewService.NavigateHomeAsync` and `WelcomeViewService.ContinueAsGuestAsync` also go to Welcome. | `UI/View/Services/Pages/LoginViewService.cs:68`; `Registration/RegistrationSuccessViewService.cs:28`; `WelcomeViewService.cs:41` | Default target `/home`. Decide what guest mode means before wiring `ContinueAsGuest`. | open |
| B8 | **Resend only restarts the countdown; no code is re-sent.** Both SMS and email confirmation pages. | `Registration/RegistrationConfirmationViewService.cs:122-127`; `RegistrationEmailConfirmationViewService.cs:119-124` | Call `StartAsync` again with the stored destination, replace the session token/code length, then restart the timer. | open |
| B9 | **Platform provider crashes on first render.** `PlatformAuthService.State` getter throws `NotImplementedException`, so `Provider: Platform` config dies inside `GoAuthenticationStateProvider`. | `Provider.Platform/Services/PlatformAuthService.cs:11` | Return `AuthState.Unauthenticated` / `null` from the stubs, or refuse the config at startup with a clear message. | open |
| B10 | **`RegistrationWaitingViewService` polling.** `OnAppResumed` is `async void`; `CheckConfirmationAsync` passes no cancellation token so a check in flight survives `StopPolling`; 24 × 5 s = 2 min hard limit then bounces to providers with a generic error. | `Registration/RegistrationWaitingViewService.cs:87-166` | Thread `_pollingCts.Token` into `IsTokenConfirmedAsync`; make the limit configurable or show a "still waiting" state instead of failing. | open |
| B11 | **Notification controller injected after creation.** `ShowAsync` adds `"Controller"` into the parameter dictionary after `ShowNotificationAsync` already built the controller; works only because the controller keeps the same dictionary reference. | `UI/Services/Notification/GizmoGoNotificationsService.cs:21-35` | Pass the controller through the base's callback mechanism, or give `ToastNotification` a cascading/injected dismiss callback. | open |
| B12 | **`DirectBranchProvider`, Home, Branches, Favorites, Account are placeholders.** Expected for `add-auth`; listed so it is not forgotten. Branch pages will throw `NotImplementedException` into the `ErrorBoundary`. | `Provider.Direct/Services/DirectBranchProvider.cs`; `UI/Pages/{Home,Branches,Favorites,Account}.razor` | — | expected |

## C. Config, security hygiene, repo hygiene

| ID | Finding | Where | Fix | Status |
|---|---|---|---|---|
| C1 | **Backend URLs baked into embedded config.** Web appsettings (uncommitted diff) points at a personal MikroTik DDNS host `e7f50eaee099.sn.mynetname.net`; MAUI appsettings bakes a LAN IP. | `Web/wwwroot/appsettings.json` (modified, unstaged); `Maui/wwwroot/appsettings.json` | Revert the Web change before committing. Source the URL from build configuration / environment (`appsettings.{Configuration}.json`, MSBuild property, or first-run setup screen) rather than a committed constant. | open |
| C2 | **Personal phone number in source.** Demo service returns `+79173380454`. | `Provider.Direct/Services/Demo/DemoRegistrationService.cs:124` | Replace with a reserved test number (e.g. `+15555550123`). | open |
| C3 | **Submodules pinned to personal / feature branches.** `mrdpod`, `telegram-verification-status`. Both are stale snapshots (see facts table). | `.gitmodules` | Repoint to `version-3` / `dev` as part of A1. | open |
| C4 | **Silent NuGet fallback when a submodule folder is missing.** `Condition="!Exists(...)"` swaps in `Gizmo.Shared 1.0.13` / `Gizmo.Web.Api.Client 1.0.12`, which lack the registration API; the resulting build errors look unrelated. | `Core/Gizmo.Go.Core.csproj:14-20`; `Provider.Direct/Gizmo.Go.Provider.Direct.csproj:19-25` | Drop the fallback and fail with an `Error` task telling the developer to `git submodule update --init`. | open |
| C5 | **Web token storage is `localStorage`.** Readable by any injected script. Standard for Blazor WASM but worth a conscious decision; MAUI uses `SecureStorage`, which is right. | `Web/Services/LocalStorageTokenStorageService.cs` | Accept, or move the refresh token to an HttpOnly cookie flow later. | decision |
| C6 | **Trimming risk in the Web publish.** `PublishTrimmed=true` while the client lib registers clients via `Assembly.ExportedTypes` + `MakeGenericMethod`; `WasmStripILAfterAOT=true` with `RunAOTCompilation=false` is a no-op. | `Web/Gizmo.Go.Web.csproj:10-13` | Verify a Release publish actually resolves every `*WebApiClient`; add a `TrimmerRootAssembly` for `Gizmo.Web.Api.Client` if not. Remove the strip flag or turn AOT on deliberately. | open |
| C7 | **`npm install` runs on every build.** Same audit-endpoint stall already recorded for the Manager UI. | `UI/Gizmo.Go.UI.csproj:37-41` | `npm config set audit false`, and/or condition the install on `package-lock.json` being newer than `node_modules`. | open |

## D. Localisation and text

| ID | Finding | Where | Fix | Status |
|---|---|---|---|---|
| D1 | **Base resx is Russian; no other locale files; language button is a hard-coded `RU` stub with no click handler.** | `UI/Resources/Resources.resx`; `UI/Components/LangButton.razor` | Decide the neutral culture (product convention is English base + locale files). Wire `LangButton` to `ILocalizationService.SetCurrentCultureAsync`. | open |
| D2 | **Hard-coded Russian aria-labels.** `LangButton.razor:1`, `Welcome.razor:21`, `RegistrationWaiting.razor:29`, `RegistrationCallVerify.razor:22`, `RegistrationSuccess.razor:26`. | as listed | Move to resources. | open |
| D3 | **Hard-coded English nav labels and page titles.** `MainLayout.razor:25,32,39,45`, `MainLayout.razor.cs:30-38`, `Home/Branches/Account.razor` titles, `NotFound.razor`. | as listed | Move to resources. | open |
| D4 | **Russian TODO comments in code.** `RegistrationPhoneViewService.cs:59,141,147`; `RegistrationPhone.razor.cs:35`; `RegistrationCallPhoneViewService.cs:81`; `RegistrationCallVerifyViewService.cs:44`; `main.js` header comment. | as listed | Translate or resolve. | open |
| D5 | **23 unused resource keys** (profile address/city/country/postcode/sex/skip, phone placeholders, two welcome chips). Leftover from a fuller profile form. | `UI/Resources/Resources.resx` | Delete, or restore the fields they belong to. | open |

## E. Style and conventions

| ID | Finding | Where | Fix | Status |
|---|---|---|---|---|
| E1 | `ct` parameter name instead of `cancellationToken`. | `Core/Services/IExternalLauncher.cs:10-12`; `Core/Services/Notification/IGizmoGoNotificationsService.cs:8`; `UI/Services/Notification/GizmoGoNotificationsService.cs:21`; `Maui/Services/MauiExternalLauncher.cs:22-24`; `Web/Services/WebExternalLauncher.cs:28-51` | Rename. | open |
| E2 | Duplicate `using Gizmo.Go.Maui.Services;`. | `Maui/MauiProgram.cs:6-7` | Remove. | open |
| E3 | `TryConvertToTelegramDeepLink` duplicated verbatim in both launchers. | `Maui/Services/MauiExternalLauncher.cs:48-63`; `Web/Services/WebExternalLauncher.cs:59-74` | Move to a Core helper. | open |
| E4 | `PlatformConfirmationService` is `public class`; every other provider service is `internal sealed`. | `Provider.Platform/Services/PlatformConfirmationService.cs:6` | Align. | open |
| E5 | Three near-identical phone view services (`RegistrationPhone`, `RegistrationEmailAddPhone`, `RegistrationCallPhone`) each carry the same `UpdatePhoneAsync` / `OnValidate` / default-country block with a `separatorBuffer = 6` TODO. | `UI/View/Services/Pages/Registration/*Phone*ViewService.cs` | Extract a shared phone-field helper once A1 settles the flow shape. | open |
| E6 | `IRegistrationSessionService` keeps flat properties "for backward compatibility" alongside `State`; interface comment says callers should migrate. | `UI/Services/Registration/IRegistrationSessionService.cs:8-10` | Finish the migration or drop the comment. | open |

## What is fine

- Core/Provider/UI layering and the `IAuthService` / `ITokenStorageService` split are sound; MAUI `SecureStorage` for tokens is correct.
- `CountdownTimer`, `OtpDigitGroup`, `PhoneInputField` debounce, and the JS resume detector (`main.js`) are careful and well commented.
- `AuthErrorCode` uses `[Name(..., nameof(Resources.*))]` per convention; all referenced UI resource keys exist.
- Build outputs under `wwwroot` are correctly untracked.

## Suggested order

1. A1 + C3 (bump submodules, re-map models) — everything else in the registration flow depends on the new shape.
2. A2 (bearer handler) and B1/B2 (refresh semantics) together — they define the auth pipeline.
3. A3 (username check) needs a server decision; park until then, but fix the Complete-failure UX.
4. A4 + C1 (backend URL sourcing, https).
5. B6, B7, B8 — small, user-visible.
6. C/D/E as a cleanup pass.
