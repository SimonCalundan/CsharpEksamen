using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Receptserver.Data;

// Used ONLY by EF Core tooling (dotnet ef migrations / database update) when
// running commands from this project. The actual runtime DbContext is created
// by the API project via dependency injection.
public class ReceptDbContextFactory : IDesignTimeDbContextFactory<ReceptDbContext>
{
    public ReceptDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReceptDbContext>();
        optionsBuilder.UseSqlite("Data Source=receptserver.db");
        return new ReceptDbContext(optionsBuilder.Options);
    }
}