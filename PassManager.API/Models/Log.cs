using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassManager.API.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public string Action { get; set; }
        public string IpAddress { get; set; } 
        public bool IsSuccess { get; set; } 
        public DateTime LogDate { get; set; }

        public User User { get; set; }
    }
}