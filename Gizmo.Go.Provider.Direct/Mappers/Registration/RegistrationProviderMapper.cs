using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Provider.Direct.Helpers;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Registration
{
    internal static class RegistrationProviderMapper
    {
        internal static RegistrationProvider Map(AvailableVerificationMethodModel source) =>
            new()
            {
                MethodId       = source.MethodId,
                Name           = source.Name,
                ChannelGuid    = source.ChannelGuid,
                CapabilityGuid = source.CapabilityGuid,
                HasChannel     = source.HasChannel,
                IsPrimary      = source.IsPrimary,
            };
    }
}
