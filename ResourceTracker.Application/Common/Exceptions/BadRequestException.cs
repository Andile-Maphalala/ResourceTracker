namespace ResourceTracker.Application.Common.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }

        public BadRequestException(IDictionary<string, string[]> errors): base("Multiple errors occurred. See error details.")
        {
            Errors = errors;
        }

        public IDictionary<string, string[]> Errors { get; set; }
    }
}

