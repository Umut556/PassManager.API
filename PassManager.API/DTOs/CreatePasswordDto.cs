using System.ComponentModel.DataAnnotations;

namespace PassManager.API.DTOs
{
    public class CreatePasswordDto
    {
        [Required]
        public int GroupId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string EncryptedPassword { get; set; }
        public string Username { get; set; }
        public string? URL { get; set; }
    }
}