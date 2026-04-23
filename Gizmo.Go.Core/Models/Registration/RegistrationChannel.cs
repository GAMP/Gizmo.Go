using System;
using Gizmo;

namespace Gizmo.Go.Core.Models.Registration
{
    public static class RegistrationChannel
    {
        public static readonly Guid Telegram = new(CommunicationChannels.Telegram);
        public static readonly Guid Sms      = new(CommunicationChannels.Sms);
        public static readonly Guid Email    = new(CommunicationChannels.Email);
    }
}
