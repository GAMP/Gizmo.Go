namespace Gizmo.Go.UI.Services
{
    public interface IPhoneValidationService
    {
        PhoneValidationResult Validate(string input, string regionCode);
        int GetMaxLength(string regionCode);
    }
}
