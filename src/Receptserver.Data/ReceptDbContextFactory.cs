using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Receptserver.Data;

// Gør at jeg kan køre EF Core tools (dotnet ef migrations / database update) 
// når jeg kører commands fra dette projekt. Den faktisk runtime DbContext bliver 
// lavet af API projektet via DI
public class ReceptDbContextFactory : IDesignTimeDbContextFactory<ReceptDbContext>
{
    public ReceptDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReceptDbContext>();
        optionsBuilder.UseSqlite("Data Source=receptserver.db");
        return new ReceptDbContext(optionsBuilder.Options);
    }
}