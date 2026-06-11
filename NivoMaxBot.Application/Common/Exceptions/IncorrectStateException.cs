namespace NivoMaxBot.Application.Common.Exceptions
{
    public class IncorrectStateException : ApplicationException
    {
        public IncorrectStateException()
        {
        }

        public IncorrectStateException(string message)
            : base(message)
        {
        }

        public IncorrectStateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
