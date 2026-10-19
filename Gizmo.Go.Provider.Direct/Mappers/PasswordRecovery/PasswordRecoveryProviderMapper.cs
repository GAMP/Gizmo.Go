using Gizmo.Go.Core.Models.PasswordRecovery;
using Gizmo.Go.Provider.Direct.Helpers;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.PasswordRecovery
{
    internal static class PasswordRecoveryProviderMapper
    {
        internal static PasswordRecoveryProvider Map(AvailableVerificationMethodModel source) =>
            new()
            {
                MethodId       = source.MethodId,
                Name           = source.Name ?? string.Empty,
                ChannelGuid    = source.ChannelGuid,
                CapabilityGuid = source.CapabilityGuid,
                Channel        = ToChannel(source.ChannelGuid),
                HasChannel     = source.HasChannel,
                IsPrimary      = source.IsPrimary,
            };

        internal static PasswordRecoveryChannel? TryGetChannel(Guid channelGuid)
        {
            if (channelGuid == new Guid(CommunicationChannels.Email))
                return PasswordRecoveryChannel.Email;
            if (channelGuid == new Guid(CommunicationChannels.Sms))
                return PasswordRecoveryChannel.Sms;
            return null;
        }

        private static PasswordRecoveryChannel ToChannel(Guid channelGuid) =>
            TryGetChannel(channelGuid) ?? PasswordRecoveryChannel.Unknown;
    }
}
