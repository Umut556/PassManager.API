using System.ComponentModel.DataAnnotations;

namespace PassManager.API.DTOs
{
    public class UserForUpdateEmailDto
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }
        public string Password { get; set; }
    }
}