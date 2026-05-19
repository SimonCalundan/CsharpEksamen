using Microsoft.EntityFrameworkCore;
using Receptserver.Core.Entities;

namespace Receptserver.Core.Persistence;

// Abstraction over the EF Core DbContext. Lets services in Core depend on
// the data layer's surface without referencing Receptserver.Data directly.
// Implemented by ReceptDbContext in Receptserver.Data.
public interface IReceptDbContext
{
    DbSet<Laegehus> Laegehuse { get; }
    DbSet<Apotek> Apoteker { get; }
    DbSet<Recept> Recepter { get; }
    DbSet<Ordination> Ordinationer { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}