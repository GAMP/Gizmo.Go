using Gizmo.Go.Core.Models.Balance;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Balance
{
    internal static class UserBalanceMapper
    {
        internal static UserBalance Map(UserBalanceExtendedModel source) =>
            new()
            {
                Balance  = source.Balance,
                Points   = source.Points,
                Deposits = source.Deposits,
            };
    }
}
