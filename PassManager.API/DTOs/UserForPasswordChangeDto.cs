using System.ComponentModel.DataAnnotations;

namespace PassManager.API.DTOs
{
    public class UserForPasswordChangeDto
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}