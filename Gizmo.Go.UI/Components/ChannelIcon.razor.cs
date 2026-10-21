using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components
{
    public partial class ChannelIcon : ComponentBase
    {
        private static readonly Guid TelegramGuid = new(CommunicationChannels.Telegram);
        private static readonly Guid FacebookMessengerGuid = new(CommunicationChannels.FacebookMessenger);
        private static readonly Guid EmailGuid = new(CommunicationChannels.Email);
        private static readonly Guid SmsGuid = new(CommunicationChannels.Sms);

        [Parameter] public Guid ChannelGuid { get; set; }

        private ChannelIconKind _kind => ChannelGuid == TelegramGuid ? ChannelIconKind.Telegram
            : ChannelGuid == FacebookMessengerGuid ? ChannelIconKind.Facebook
            : ChannelGuid == EmailGuid ? ChannelIconKind.Email
            : ChannelGuid == SmsGuid ? ChannelIconKind.Sms
            : ChannelIconKind.Globe;

        private enum ChannelIconKind { Telegram, Facebook, Email, Sms, Globe }
    }
}
