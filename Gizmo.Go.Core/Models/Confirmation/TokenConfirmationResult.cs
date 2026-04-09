namespace Gizmo.Go.Core.Models.Confirmation;

public sealed class TokenConfirmationResult
{
    public const string Success = "Success";
    public const string Failure = "Failure";
    public const string InvalidToken = "Invalid token";
    public const string InvalidTokenInput = "Invalid token input";
    public const string ExpiredToken = "Expired token";
    public const string UsedToken = "Used token";
    public const string RevokedToken = "Revoked token";
    public const string InvalidVerification = "Invalid verification";
    public const string AlreadyVerified = "Already verified";
    public const string InvalidInput = "Invalid input";
    public const string PartialSuccess = "Partial success";
    public const string InvalidConfirmationCode = "Invalid confirmation code";
    public const string Unknown = "Unknown";

    public required string Result { get; init; }
}