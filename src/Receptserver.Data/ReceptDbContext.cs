using Microsoft.EntityFrameworkCore;
using Receptserver.Core.Entities;
using Receptserver.Core.Persistence;

namespace Receptserver.Data;

public class ReceptDbContext : DbContext, IReceptDbContext
{
    public ReceptDbContext(DbContextOptions<ReceptDbContext> options) : base(options)
    {
    }

    public DbSet<Laegehus> Laegehuse => Set<Laegehus>();
    public DbSet<Apotek> Apoteker => Set<Apotek>();
    public DbSet<Recept> Recepter => Set<Recept>();
    public DbSet<Ordination> Ordinationer => Set<Ordination>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Laegehus>(entity =>
        {
            entity.HasIndex(l => l.Ydernummer).IsUnique();
            entity.Property(l => l.Ydernummer).HasMaxLength(50).IsRequired();
            entity.Property(l => l.Navn).HasMaxLength(200).IsRequired();
            entity.Property(l => l.Adresse).HasMaxLength(300).IsRequired();
        });

        modelBuilder.Entity<Apotek>(entity =>
        {
            entity.Property(a => a.Navn).HasMaxLength(200).IsRequired();
            entity.Property(a => a.Adresse).HasMaxLength(300).IsRequired();
        });

        modelBuilder.Entity<Recept>(entity =>
        {
            entity.Property(r => r.Ydernummer).HasMaxLength(50).IsRequired();
            entity.Property(r => r.CprNummer).HasMaxLength(11).IsRequired();

            entity.HasIndex(r => r.CprNummer);

            entity.HasOne(r => r.TilknyttetApotek)
                .WithMany()
                .HasForeignKey(r => r.TilknyttetApotekId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(r => r.Ordinationer)
                .WithOne(o => o.Recept!)
                .HasForeignKey(o => o.ReceptId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Ordination>(entity =>
        {
            entity.Property(o => o.Laegemiddel).HasMaxLength(200).IsRequired();
            entity.Property(o => o.Dosis).HasMaxLength(300).IsRequired();
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Laegehus>().HasData(
            new Laegehus { Id = 1, Ydernummer = "012345", Navn = "Lægerne i Aarhus C", Adresse = "Banegårdsgade 1, 8000 Aarhus" },
            new Laegehus { Id = 2, Ydernummer = "054321", Navn = "Risskov Lægehus", Adresse = "Skovvejen 12, 8240 Risskov" },
            new Laegehus { Id = 3, Ydernummer = "067890", Navn = "Viby Lægehus", Adresse = "Skanderborgvej 200, 8260 Viby" }
        );

        modelBuilder.Entity<Apotek>().HasData(
            new Apotek { Id = 1, Navn = "Aarhus Apotek", Adresse = "Store Torv 3, 8000 Aarhus" },
            new Apotek { Id = 2, Navn = "Risskov Apotek", Adresse = "Stadionalle 1, 8240 Risskov" },
            new Apotek { Id = 3, Navn = "Viby Apotek", Adresse = "Skanderborgvej 220, 8260 Viby" }
        );
    }
}