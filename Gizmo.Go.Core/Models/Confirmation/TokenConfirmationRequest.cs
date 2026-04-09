namespace Gizmo.Go.Core.Models.Confirmation;

public sealed class TokenConfirmationRequest
{
    public required string Token { get; set; }
    
    public required string ConfirmationCode { get; set; }
}