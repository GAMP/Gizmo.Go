using Gizmo.Go.Core.Models.Registration;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class RegistrationProfileViewState : ViewStateBase
    {
        public string Username { get; internal set; } = string.Empty;
        public string Email { get; internal set; } = string.Empty;
        public string FirstName { get; internal set; } = string.Empty;
        public string LastName { get; internal set; } = string.Empty;
        public DateTime? BirthDate { get; internal set; }
        public string Address { get; internal set; } = string.Empty;
        public string City { get; internal set; } = string.Empty;
        public string Country { get; internal set; } = string.Empty;
        public string PostCode { get; internal set; } = string.Empty;
        public string Phone { get; internal set; } = string.Empty;
        public string MobilePhone { get; internal set; } = string.Empty;
        public UserSex Sex { get; internal set; } = UserSex.Unspecified;

        public bool IsSubmitting { get; internal set; }
        public RegistrationCompleteResultCode? ErrorCode { get; internal set; }
        public bool CanSubmit => Username.Length > 0;
    }
}
