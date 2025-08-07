using System.ComponentModel.DataAnnotations;

namespace PassManager.API.DTOs
{
    public class CreateGroupDto
    {
        [Required]
        public string GroupName { get; set; }
    }
}