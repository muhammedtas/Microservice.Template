using System.ComponentModel.DataAnnotations;

namespace IntradayDashboard.WebApi.Dto
{
    public class UserForLoginDto
    {
        [Required]
        public string UserName {get;set;}
        [Required]
        [StringLength(10,MinimumLength = 5, ErrorMessage ="You must specify a password between 10 and 5 characters")]
        public string Password {get;set;}
    }
}