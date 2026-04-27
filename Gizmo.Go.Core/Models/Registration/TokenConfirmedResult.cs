namespace Gizmo.Go.Core.Models.Registration
{
    public sealed class TokenConfirmedResult
    {
        public bool IsConfirmed { get; init; }
        public string? Phone { get; init; }
    }
}
