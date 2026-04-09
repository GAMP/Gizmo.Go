using Gizmo.Go.Core.Models.Registration;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Registration;

internal static class RegistrationProviderMapper
{
    public static RegistrationProvider Map(VerificationProviderModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new RegistrationProvider
        {
            PublicId = model.PublicId,
            Name = model.Name,
            ChannelGuid = model.ChannelGuid,
            CanDispatchCode = model.CanDispatchCode,
            CanProvideEmail = model.CanProvideEmail,
            CanProvidePhone = model.CanProvidePhone,
            CanRedirect = model.CanRedirect,
            HasChannel = model.HasChannel
        };
    }    
}