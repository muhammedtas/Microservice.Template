using System.Collections.Generic;

namespace IntradayDashboard.WebApi.Dto
{
    public class RoleDto
    {
        public ICollection<UserRoleDto> UserRoles { get; set; }
    }
}