using Microsoft.EntityFrameworkCore;
using Receptserver.Api.Middleware;
using Receptserver.Core.Persistence;
using Receptserver.Core.Services;
using Receptserver.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("ReceptDb")
    ?? "Data Source=receptserver.db";

builder.Services.AddDbContext<ReceptDbContext>(opts => opts.UseSqlite(connectionString));

// Bind IReceptDbContext til samme instans som ReceptDbContext i samme scope.
// Services i Core afhænger af interfacet; ASP.NET resolver det til den konkrete DbContext.
builder.Services.AddScoped<IReceptDbContext>(sp => sp.GetRequiredService<ReceptDbContext>());

builder.Services.AddScoped<IRecipeService, RecipeService>();

var app = builder.Build();

// Kør pending migrations automatisk ved opstart (prototype-bekvemt).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReceptDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<DomainExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
