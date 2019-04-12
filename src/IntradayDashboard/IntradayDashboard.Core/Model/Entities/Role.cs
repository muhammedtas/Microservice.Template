using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace IntradayDashboard.Core.Model.Entities
{
    public class Role : IdentityRole<int>
    {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}