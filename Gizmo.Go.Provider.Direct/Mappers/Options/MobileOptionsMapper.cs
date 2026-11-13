using Gizmo.Go.Core.Models.Options;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Options
{
    internal static class MobileOptionsMapper
    {
        internal static GoMobileOptions Map(MobileOptionsModel source, string baseUrl) =>
            new()
            {
                IsRegistrationEnabled     = source.IsRegistrationEnabled,
                IsPasswordRecoveryEnabled = source.IsPasswordRecoveryEnabled,
                LogoUrl                   = source.LogoImageGuid is { } guid
                    ? $"{baseUrl.TrimEnd('/')}/files/{guid:N}"
                    : null,
                PreferredLanguage         = source.PreferredLanguage,
                CountryCode               = source.CountryCode,
                Currency                  = source.Currency,
                PasswordValidation        = source.PasswordValidation,
            };
    }
}
