using System.Globalization;
using Gizmo.Go.Core.Models.Branch;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.Provider.Direct.Mappers.Branch
{
    internal static class BranchMapper
    {
        internal static Core.Models.Branch.Branch Map(UserBranchModel source, ILogger logger) =>
            new()
            {
                Id                   = source.Id,
                Name                 = source.Name,
                Country              = source.Country,
                City                 = source.City,
                Address              = source.Address,
                PostalCode           = source.PostalCode,
                Region               = source.Region,
                Phone                = source.Phone,
                Email                = source.Email,
                WebSite              = source.WebSite,
                Info                 = source.Info,
                TimeZone             = source.TimeZone,
                BusinessStartWeekDay = source.BusinessStartWeekDay,
                BusinessEndWeekDay   = source.BusinessEndWeekDay,
                BusinessDayStart     = ParseTime(source, source.BusinessDayStart, logger),
                BusinessDayEnd       = ParseTime(source, source.BusinessDayEnd, logger),
            };

        internal static BranchDetails Map(UserBranchDetailsModel source, ILogger logger) =>
            new()
            {
                Branch         = Map(source.Branch, logger),
                TotalComputers = source.TotalComputers,
                FreeComputers  = source.FreeComputers,
                HostGroups     = source.HostGroups.ToList(),
            };

        // an empty or unparsable time means no business hours; an unparsable one is logged
        private static TimeOnly? ParseTime(UserBranchModel source, string? value, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (TimeOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
                return time;

            logger.LogWarning("Cannot parse the business time {Value} of the branch {BranchId}.", value, source.Id);
            return null;
        }
    }
}
