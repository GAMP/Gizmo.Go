using Gizmo.Go.Core.Models.Ladder;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.Ladder
{
    internal static class LadderLevelMapper
    {
        /// <summary>
        /// Maps the current level of the standing; null when the standing has no matching level.
        /// </summary>
        internal static LadderLevel? Map(LadderStandingModel? source, string baseUrl)
        {
            if (source is null)
                return null;

            for (var i = 0; i < source.Levels.Count; i++)
            {
                var level = source.Levels[i];

                if (level.Rank != source.CurrentRank)
                    continue;

                return new LadderLevel
                {
                    Ordinal   = i + 1,
                    Name      = level.Name,
                    EmblemUrl = level.EmblemGuid is { } guid
                        ? $"{baseUrl.TrimEnd('/')}/files/{guid:N}"
                        : null,
                };
            }

            return null;
        }
    }
}
