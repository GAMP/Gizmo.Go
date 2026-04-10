using Gizmo.Go.Core.Models.Registration;

namespace Gizmo.Go.UI.Helpers;

public static class RegistrationCompleteErrorHelper
{
    public static string GetErrorMessage(RegistrationCompleteResultCode code) => code switch
    {
        RegistrationCompleteResultCode.InvalidToken      => "Недействительный токен. Начните регистрацию заново.",
        RegistrationCompleteResultCode.InvalidTokenInput => "Недействительный токен. Начните регистрацию заново.",
        RegistrationCompleteResultCode.ExpiredToken      => "Срок действия токена истёк. Начните регистрацию заново.",
        RegistrationCompleteResultCode.UsedToken         => "Токен уже был использован.",
        RegistrationCompleteResultCode.RevokedToken      => "Токен недействителен.",
        _                                                => "Произошла ошибка. Попробуйте снова.",
    };
}
