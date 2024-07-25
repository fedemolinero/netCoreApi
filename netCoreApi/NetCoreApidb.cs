using Microsoft.EntityFrameworkCore;

// The preceding code defines the database context, which is the main class that coordinates 
// Entity Framework functionality for a data model. This class derives from the Microsoft.EntityFrameworkCore.DbContext class.

class NetCoreApiDb : DbContext
{
    public NetCoreApiDb(DbContextOptions<NetCoreApiDb> options)
        : base(options) { }

    public DbSet<NetCoreApi> NetCoreApis => Set<NetCoreApi>();
}