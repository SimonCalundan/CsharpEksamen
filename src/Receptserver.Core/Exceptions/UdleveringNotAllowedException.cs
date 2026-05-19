namespace Receptserver.Core.Exceptions;

public class UdleveringNotAllowedException : DomainException
{
    public int OrdinationId { get; }

    public UdleveringNotAllowedException(int ordinationId, string reason)
        : base($"Udlevering på ordination {ordinationId} ikke tilladt: {reason}")
    {
        OrdinationId = ordinationId;
    }
}