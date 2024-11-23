
    namespace AccessControlApi.Domain.Exceptions;

    public class AccessControlException : Exception
    {
        public AccessControlException(string message) : base(message)
        {
        }

        public AccessControlException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
