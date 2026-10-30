using Gizmo.Go.Core.Models;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Common
{
    internal static class MobileOptionsMapper
    {
        internal static MobileOptions Map(MobileOptionsModel source, string baseUrl) =>
            new()
            {
                IsRegistrationEnabled     = source.IsRegistrationEnabled,
                IsPasswordRecoveryEnabled = source.IsPasswordRecoveryEnabled,
                LogoUrl                   = source.LogoImageGuid is { } guid
                    ? $"{baseUrl.TrimEnd('/')}/files/{guid:N}"
                    : null,
                PreferredLanguage         = source.PreferredLanguage,
                CountryCode               = source.CountryCode,
                PasswordValidation        = source.PasswordValidation,
            };
    }
}
