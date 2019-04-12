using IntradayDashboard.Core.Model.Interfaces;

namespace IntradayDashboard.Core.Model.Entities
{
    public class Tenant : BaseEntity, IMultiTenant
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
    }
}