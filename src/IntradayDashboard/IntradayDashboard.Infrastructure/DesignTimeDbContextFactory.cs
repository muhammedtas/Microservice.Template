using System.IO;
using IntradayDashboard.Infrastructure.Data.DataContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace IntradayDashboard.Infrastructure
{
    public class DesignTimeDbContextFactory: IDesignTimeDbContextFactory<IntradayDbContext>
    {
        // public readonly DbSettings _appSettings;

        // public DesignTimeDbContextFactory(IOptions<DbSettings> appSettings)
        // {
        //     _appSettings.ConnectionString = appSettings.Value.ConnectionString;
        // }
        public IntradayDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<IntradayDbContext>();

            var connectionString = "Server=localhost,1420; Database=IntradayDashboard; User ID=sa; Password=GitGood*0987654321;";

            builder.UseSqlServer(connectionString);

            return new IntradayDbContext(builder.Options);
        }

        
       
        
    }
    public class DbSettings {
        public string ConnectionString { get; set; }
    }
}