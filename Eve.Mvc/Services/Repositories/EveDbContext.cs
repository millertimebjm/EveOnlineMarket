
public class EveDbContext : DbContext
{
    public EveDbContext(DbContextOptions options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to postgres with connection string from app settings
        options.UseNpgsql(options.GetConnectionString());
    }


}