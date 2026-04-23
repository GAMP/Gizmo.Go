namespace Gizmo.Go.UI.Services.Registration
{
    public interface IPhoneValidationService
    {
        PhoneValidationResult Validate(string input, string regionCode);
        int GetMaxLength(string regionCode);
        string MaskPhone(string phone);
    }
}
