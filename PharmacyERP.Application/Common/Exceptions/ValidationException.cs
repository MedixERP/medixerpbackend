namespace PharmacyERP.Application.Common.Exceptions;

public class ValidationException : BusinessException
{
    public ValidationException(string message)
        : base(message, 400)
    {
    }
}