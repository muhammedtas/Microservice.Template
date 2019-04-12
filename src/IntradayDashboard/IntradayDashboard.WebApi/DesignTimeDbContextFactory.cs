using System.IO;
using IntradayDashboard.Infrastructure.Data.DataContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IntradayDashboard.WebApi
{
    public class DesignTimeDbContextFactory: IDesignTimeDbContextFactory<IntradayDbContext>
    {
        public IntradayDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<IntradayDbContext>();

            var connectionString = configuration.GetConnectionString("MSSQLConnection");

            builder.UseSqlServer(connectionString);

            return new IntradayDbContext(builder.Options);
        }
       
        
    }
}