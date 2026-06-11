namespace NivoMaxBot.Application.Common.Exceptions
{
    public class BusinessRuleViolationException : ApplicationException
    {
        public BusinessRuleViolationException()
        {
        }

        public BusinessRuleViolationException(string message)
            : base(message)
        {
        }

        public BusinessRuleViolationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
