using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Provider.Direct.Helpers;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Registration
{
    internal static class RegistrationCompleteRequestMapper
    {
        public static RegistrationCompleteModel MapComplete(RegistrationCompleteRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            return new RegistrationCompleteModel
            {
                Token          = request.Token ?? string.Empty,
                Password       = request.Password,
                Profile        = RegistrationProfileMapper.Map(request.Profile),
                AgreementStates = MapAgreementStates(request.AgreementStates)
            };
        }

        private static List<UserAgreementModelState> MapAgreementStates(
            IReadOnlyList<RegistrationAgreementChoice> choices)
            => choices is null
                ? new()
                : choices.Select(c => new UserAgreementModelState
                {
                    UserId = 0,
                    UserAgreementId = c.AgreementId,
                    AcceptState = (UserAgreementAcceptState)c.AcceptState
                }).ToList();
    }
}
