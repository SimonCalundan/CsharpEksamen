namespace Receptserver.Core.Exceptions;

public class NotFoundException : DomainException
{
    public string EntityType { get; }
    public string Identifier { get; }

    public NotFoundException(string entityType, string identifier)
        : base($"{entityType} med identifier '{identifier}' blev ikke fundet.")
    {
        EntityType = entityType;
        Identifier = identifier;
    }
}