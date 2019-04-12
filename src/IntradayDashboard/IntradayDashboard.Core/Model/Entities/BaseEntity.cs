using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IntradayDashboard.Core.Model.Interfaces;

namespace IntradayDashboard.Core.Model.Entities
{
    public abstract class BaseEntity : IBaseEntity
    {
        [Key]
        [Column(Order = 1)]
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        
    }
}