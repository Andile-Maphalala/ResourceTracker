
namespace ResourceTracker.Application.Common.Search
{
    internal static class SearchExtensions
    {
        internal static IEnumerable<string> GetSearchTerms(this string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return Enumerable.Empty<string>();

            return search
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(term => term.Trim())
                .Where(term => term.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }
    }
}
