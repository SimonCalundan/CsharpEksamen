using Microsoft.EntityFrameworkCore;
using Receptserver.Core.Entities;

namespace Receptserver.Core.Persistence;

// Abstraktion ovenpå EF Core DbContext fra Data-laget. Lar services i Core 
// afhænge af interfacet, uden at referere til Receptserver.Data direkte.
public interface IReceptDbContext
{
    DbSet<Laegehus> Laegehuse { get; }
    DbSet<Apotek> Apoteker { get; }
    DbSet<Recept> Recepter { get; }
    DbSet<Ordination> Ordinationer { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}