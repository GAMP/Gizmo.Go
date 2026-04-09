namespace Gizmo.Go.Core.Models.Registration;

public sealed class RegistrationProfile
{
    public required string Username { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? PostCode { get; set; }

    public string? Phone { get; set; }

    public string? MobilePhone { get; set; }

    public UserSex Sex { get; set; }
}
