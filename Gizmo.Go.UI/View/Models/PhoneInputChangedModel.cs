namespace Gizmo.Go.UI.View.Models
{
    public sealed class PhoneInputChangedModel
    {
        public string Input { get; set; } = string.Empty;

        public string CountryIso2 { get; set; } = string.Empty;

        public string CountryName { get; set; } = string.Empty;

        public string DialCode { get; set; } = string.Empty;

        public string E164 { get; set; } = string.Empty;

        public bool IsValid { get; set; }

        public string? ValidationError { get; set; }
    }
}
