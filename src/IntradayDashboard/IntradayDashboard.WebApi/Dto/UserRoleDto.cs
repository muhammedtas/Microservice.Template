namespace IntradayDashboard.WebApi.Dto
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public UserDto User { get; set; }
        public RoleDto Role { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}