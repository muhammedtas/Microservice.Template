using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using IntradayDashboard.Core.Model.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IntradayDashboard.Core.Model.Entities
{
    [Table("User")]
    public class User : IdentityUser<int>, IMultiTenant, IAuditEntity
    {
        
        /// In this table we going to add every sort of authentication process as cyrpted values.
        // For example; for epiaş, teiaş, ipop, windows etc usernames and password will may saved to here as objects
        public User()
        {
            Guid = System.Guid.Empty.ToString();
        }

        public string Firstname {get;set;}
        public string Lastname {get;set;}
        public string NationalIdentificationNumber {get;set;}
        public string Gender { get; set; }
        public string Guid {get;set;}
        public string KnownAs { get; set; }
        public string Phone {get;set;}
        public DateTime DateOfBirth { get; set; }
        public DateTime LastEnterance { get; set; }
        public string Title {get;set;}
        public ICollection<UserRole> UserRoles {get;set;}
        public int TenantId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}