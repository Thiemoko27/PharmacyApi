
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PharmacyApi.Models;

namespace PharmacyApi.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataBaseContext>
{
    public DataBaseContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if(string.IsNullOrEmpty(connectionString)) {
            throw new InvalidOperationException("Connection string 'DefaultConnection' Not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<DataBaseContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new DataBaseContext(optionsBuilder.Options);
    }
}

public class DataBaseContext : DbContext
{
    public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }
    public DbSet<Drug> Drugs { get; set; } = null!;
}

public class UserDataBaseContext : DbContext
{
    public UserDataBaseContext(DbContextOptions<UserDataBaseContext> options) : base(options) { }
    public DbSet<User> Users {get; set; } = null!;
}
