using System;
using System.ComponentModel.DataAnnotations;

namespace IntradayDashboard.WebApi.Dto
{
    public class UserRegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(10,MinimumLength = 5, ErrorMessage ="You must specify a password between 10 and 5 characters")]
        public string  Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        //[Required]
        [Required]
        public DateTime DateOfBirth { get; set; }

        public DateTime Created {get;set;}

        public DateTime LastEnterance { get; set; }

        public UserRegisterDto() {
            Created = DateTime.Now;
            LastEnterance = DateTime.Now;
        }
    }
}