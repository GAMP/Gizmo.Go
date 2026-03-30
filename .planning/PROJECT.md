# Gizmo.Go — Phone Input (CreateAccount)

## What This Is

Добавление международного ввода номера телефона на страницу CreateAccount в Gizmo.Go — мобильном клиенте игрового зала. Пользователь выбирает страну через дропдаун с флагами, вводит номер, приложение собирает строку в формате E.164 (`+79161234567`). Одновременно переносим состояние со страницы в ViewState/ViewService по принятому в проекте паттерну.

## Core Value

Пользователь из любой страны может ввести свой номер телефона — и приложение собирает правильный E.164-номер без участия бэкенда.

## Requirements

### Validated

- ✓ Blazor WebAssembly + MAUI хост с общим UI в `Gizmo.Go.UI` — existing
- ✓ ViewState/ViewService паттерн через `Gizmo.UI` NuGet (`ValidatingViewStateBase`, `[Register]`) — existing
- ✓ Страница CreateAccount с полем телефона и чекбоксом terms — existing
- ✓ Навигация CreateAccount → Confirmation с передачей phone через query string — existing

### Active

- [ ] ViewState (`CreateAccountViewState`) и ViewService (`CreateAccountViewService`) для страницы CreateAccount
- [ ] Дропдаун выбора страны с флагом и кодом (intl-tel-input или аналог через JS Interop)
- [ ] Поле ввода локальной части номера (без кода страны)
- [ ] Сборка E.164-строки: `+{countryCode}{localNumber}`
- [ ] Валидация: номер соответствует формату выбранной страны (приоритет — переиспользование существующих NuGet-валидаторов)
- [ ] Submit передаёт в Confirmation E.164-строку вместо текущего `+7{_phone}`
- [ ] Кнопка Submit disabled если форма невалидна (телефон + terms)

### Out of Scope

- Backend OTP / SMS — пока только фронт, интеграция отдельная задача
- libphonenumber-csharp на бэкенде — только frontend-валидация в этом scope
- Шифрование PII в базе — backend задача
- Rate limiting — backend задача
- Confirmation, CreatePassword ViewState/ViewService — отдельный tech debt, в этот scope не входит

## Context

**Текущее состояние CreateAccount:**
- Состояние (`_phone`, `_termsAccepted`) хранится прямо в компоненте — tech debt
- Валидация захардкожена под Россию: `_phone.Length == 10 && _phone.All(char.IsDigit)`
- Код страны `+7` захардкожен прямо в Razor (визуально) и в навигации (`+7{_phone}`)
- Нет ViewState/ViewService — исключение среди auth-страниц (Login уже на паттерне)

**Паттерн проекта (ViewState/ViewService):**
- `*ViewState` : `ValidatingViewStateBase` (или `ViewStateBase`), `[Register]`, свойства `internal set`
- `*ViewService` : `ValidatingViewStateServiceBase<TViewState>`, `[Register]`, `[Route("/create-account")]`
- Razor-компонент: `OnInitialized` → `SubscribeChange(ViewState)`, `Dispose` → `UnsubscribeChange(ViewState)`
- Мутации только в ViewService, компонент не трогает ViewState напрямую

**NuGet-пакеты для переиспользования:**
- `Gizmo.UI` — `ValidatingViewStateBase`, `ValidatingViewStateServiceBase`, `[Register]`
- Нужно исследовать: есть ли готовые валидационные атрибуты или сервисы для телефона в `Gizmo.UI` / `Gizmo.Shared`

**Интеграция JS:**
- Проект использует JS Interop через `IJSRuntime` / `JSRuntimeService` (паттерн из `Gizmo.UI`)
- Webpack pipeline: `src/js/main.js` → `wwwroot/js/app.js`

## Constraints

- **Tech Stack**: Blazor WASM + MAUI, C# / Razor, Webpack/SCSS — не менять
- **Pattern**: ViewState/ViewService из `Gizmo.UI` — обязательно, не изобретать своё
- **Reuse First**: использовать существующие NuGet валидаторы если есть, иначе — JS Interop (intl-tel-input)
- **Frontend Only**: никакого кода бэкенда в этом scope
- **Asset Pipeline**: не редактировать `wwwroot/css/` и `wwwroot/js/` напрямую — только через webpack

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| intl-tel-input через JS Interop | Стандартная библиотека (14k★), готовый дропдаун с флагами, возвращает E.164 | — Pending (зависит от research) |
| Переиспользовать NuGet валидаторы | Проект уже имеет `ValidatingViewStateBase` с DataAnnotations; нужно проверить есть ли phone атрибуты | — Pending |

## Evolution

This document evolves at phase transitions and milestone boundaries.

**After each phase transition** (via `/gsd:transition`):
1. Requirements invalidated? → Move to Out of Scope with reason
2. Requirements validated? → Move to Validated with phase reference
3. New requirements emerged? → Add to Active
4. Decisions to log? → Add to Key Decisions
5. "What This Is" still accurate? → Update if drifted

**After each milestone** (via `/gsd:complete-milestone`):
1. Full review of all sections
2. Core Value check — still the right priority?
3. Audit Out of Scope — reasons still valid?
4. Update Context with current state

---
*Last updated: 2026-03-30 after initialization*
