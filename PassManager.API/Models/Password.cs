using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassManager.API.Models
{
    public class Password
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Group")]
        public int GroupId { get; set; }

        public string Title { get; set; }
        public string EncryptedPassword { get; set; }
        public string Username { get; set; }
        public string? URL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Group Group { get; set; }
    }
}