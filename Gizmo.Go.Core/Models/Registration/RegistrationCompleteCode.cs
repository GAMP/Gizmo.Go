namespace Gizmo.Go.Core.Models.Registration
{
    public enum RegistrationCompleteCode
    {
        Success,
        Failure,
        InvalidToken,
        ExpiredToken,
        UsedToken,
        InvalidInput,
        NoUserGroup,
        NonUniqueInput,
        Unknown
    }
}
