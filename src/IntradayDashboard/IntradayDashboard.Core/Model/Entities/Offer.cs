using System;
using System.ComponentModel.DataAnnotations.Schema;
using IntradayDashboard.Core.Model.Interfaces;

namespace IntradayDashboard.Core.Model.Entities
{
    [Table("Offer")]
    public class Offer: BaseEntity, IMultiTenant, IAuditEntity
    {
        public string Data { get; set; }
        public int Value  { get; set; }
        public int TenantId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}