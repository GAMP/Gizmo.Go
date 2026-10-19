namespace Gizmo.Go.Core.Configuration
{
    /// <summary>
    /// Temporary source of registration options, default-group required info and password rules.
    /// Replaced by open API endpoints in a separate server task.
    /// </summary>
    public sealed class RegistrationStubSettings
    {
        public bool IsRegistrationEnabled { get; set; } = true;
        public bool IsDirectRegistrationEnabled { get; set; }
        public bool IsPasswordRecoveryEnabled { get; set; } = true;
        public RegistrationRequiredInfoStub RequiredInfo { get; set; } = new();
    }

    public sealed class RegistrationRequiredInfoStub
    {
        public bool FirstName { get; set; } = true;
        public bool LastName { get; set; } = true;
        public bool BirthDate { get; set; } = true;
        public bool Sex { get; set; } = true;
        public bool Email { get; set; } = true;
        public bool Mobile { get; set; } = true;
        public bool Phone { get; set; } = true;
        public bool Country { get; set; } = true;
        public bool Address { get; set; } = true;
        public bool City { get; set; } = true;
        public bool PostCode { get; set; } = true;
        public bool State { get; set; } = true;
    }
}
