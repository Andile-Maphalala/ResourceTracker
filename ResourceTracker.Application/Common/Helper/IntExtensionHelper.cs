

namespace ResourceTracker.Application.Common.Helper
{
    public static class IntExtensionHelper
    {
        public static bool IsNullOrEmpty(this int? value)
        {
            return !value.HasValue || value.Value == 0;
        }
    }
}
