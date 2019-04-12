using System;
using System.Collections.Generic;
using System.Text;

namespace IntradayDashboard.Core.Model.Interfaces
{
    public interface IAuditEntity
    {
        string CreatedBy { get; set; }
        DateTime? CreatedOn { get; set; }
        string UpdatedBy { get; set; }
        DateTime? UpdatedOn { get; set; }

        string DeletedBy { get; set; }
        DateTime? DeletedOn { get; set; }
    }
}
