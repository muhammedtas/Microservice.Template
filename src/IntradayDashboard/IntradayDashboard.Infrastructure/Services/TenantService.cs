using IntradayDashboard.Core.Model.Entities;
using IntradayDashboard.Core.Services.Interfaces;
using IntradayDashboard.Infrastructure.Data.DataContexts;
using IntradayDashboard.Infrastructure.Repositories.Database;

namespace IntradayDashboard.Infrastructure.Services
{
    public class TenantService: Repository<Tenant>, ITenantService
    {
        public TenantService(IntradayDbContext context) : base(context)
        {
        }
        
    }
}