using System.ComponentModel.DataAnnotations;

namespace PassManager.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Group> Groups { get; set; }
        public ICollection<Log> Logs { get; set; }
    }
}