using ComfyBot.Data.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ComfyBot.Data.Database;

public class DataContext : DbContext
{
    private readonly DataSettings settings;

    public DataContext(DbContextOptions<DataContext> options, IOptions<DataSettings> settings) : base(options)
    {
        this.settings = settings.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(this.settings.DatabaseConnection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }
}