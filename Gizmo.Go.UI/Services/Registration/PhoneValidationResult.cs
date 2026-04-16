namespace Gizmo.Go.UI.Services.Registration
{
    public readonly struct PhoneValidationResult
    {
        public bool IsValid { get; init; }
        public string E164 { get; init; }
        public string FormattedNational { get; init; }

        public static PhoneValidationResult Invalid =>
            new() { E164 = string.Empty, FormattedNational = string.Empty };
    }
}
