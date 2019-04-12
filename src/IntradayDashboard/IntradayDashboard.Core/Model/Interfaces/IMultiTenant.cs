using System;

namespace IntradayDashboard.Core.Model.Interfaces
{
  
    public interface IMultiTenant
    {
         int TenantId { get; set; }
    }
    
}