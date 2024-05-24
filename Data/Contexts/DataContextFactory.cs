using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
        {
            public DataContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
                optionsBuilder.UseSqlServer("Server=tcp:ejm-win23-jsbackend-subscribers.database.windows.net,1433;Initial Catalog=ejm-files-database;Persist Security Info=False;User ID=ejm-win23-jsbackend;Password=Bytmig123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

                return new DataContext(optionsBuilder.Options);
            }
        }
}
