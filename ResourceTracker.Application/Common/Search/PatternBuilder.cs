using ResourceTracker.Application.Models.Enums;

namespace ResourceTracker.Application.Common.Search
{
    public static class PatternBuilder
    {
        public static string BuildLikePattern(string term, SearchMatchType matchType)
        {
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
