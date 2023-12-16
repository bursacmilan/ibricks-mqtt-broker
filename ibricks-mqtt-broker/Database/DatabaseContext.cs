using System.Reflection;
using System.Text.Json;
using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Model.DeviceState;
using Microsoft.EntityFrameworkCore;

namespace ibricks_mqtt_broker.Database;

public class DatabaseContext : DbContext
{
    public required DbSet<Cello> Cellos { get; set; }

    private readonly string _dbPath;
    
    public DatabaseContext()
    {
        var assemblyPath = Assembly.GetExecutingAssembly().Location;
        var directory = Path.GetDirectoryName(assemblyPath);
        if (directory == null)
            throw new Exception("Path is NULL");

        _dbPath = Path.Combine(directory, "database.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cello>()
            .Property(b => b.ClimateStates)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<Dictionary<int, ClimateState>>(v, JsonSerializerOptions.Default) ??
                     new Dictionary<int, ClimateState>());
        
        modelBuilder.Entity<Cello>()
            .Property(b => b.CoverStates)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<Dictionary<int, CoverState>>(v, JsonSerializerOptions.Default) ??
                     new Dictionary<int, CoverState>());
        
        modelBuilder.Entity<Cello>()
            .Property(b => b.DimmerStates)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<Dictionary<int, DimmerState>>(v, JsonSerializerOptions.Default) ??
                     new Dictionary<int, DimmerState>());
        
        modelBuilder.Entity<Cello>()
            .Property(b => b.MeteoStates)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<Dictionary<int, MeteoState>>(v, JsonSerializerOptions.Default) ??
                     new Dictionary<int, MeteoState>());
        
        modelBuilder.Entity<Cello>()
            .Property(b => b.RelayStates)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<Dictionary<int, RelayState>>(v, JsonSerializerOptions.Default) ??
                     new Dictionary<int, RelayState>());
        
        modelBuilder.Entity<Cello>()
            .Property(b => b.HardwareInfo)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<IbricksHardwareInfo>(v, JsonSerializerOptions.Default));
    }
}