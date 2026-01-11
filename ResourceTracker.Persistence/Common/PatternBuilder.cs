using ResourceTracker.Application.Models.Enums;

namespace ResourceTracker.Persistence.Common
{
    public static class PatternBuilder
    {
        public static string BuildLikePattern(string term, SearchMatchType matchType, bool ignoreCase = true)
        {
            if(ignoreCase)
            {
                term = term.ToLowerInvariant();
            }
            switch(matchType)
            {
                case SearchMatchType.StartsWith:
                    return $"{term}%";
                case SearchMatchType.EndsWith:
                    return $"%{term}";
                case SearchMatchType.Contains:
                default:
                    return $"%{term}%";
            }
        }
    }
}
