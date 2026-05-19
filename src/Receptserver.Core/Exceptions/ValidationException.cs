namespace Receptserver.Core.Exceptions;

public class ValidationException : DomainException
{
    public string Field { get; }

    public ValidationException(string field, string message)
        : base($"Ugyldig værdi for '{field}': {message}")
    {
        Field = field;
    }
}