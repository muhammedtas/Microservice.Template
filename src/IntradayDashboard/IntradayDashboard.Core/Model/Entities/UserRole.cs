using IntradayDashboard.Core.Model.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IntradayDashboard.Core.Model.Entities
{
    public class UserRole : IdentityUserRole<int>, IBaseEntity
    {   
        public int Id { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}