using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntradayDashboard.Core.Model.Interfaces
{
    public interface IBaseEntity
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        int Id { get; set; }
        string CreatedBy { get; set; }
        bool IsDeleted { get; set; }
    }
}