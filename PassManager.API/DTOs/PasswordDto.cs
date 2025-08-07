namespace PassManager.API.DTOs
{
    public class PasswordDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Title { get; set; }
        public string EncryptedPassword { get; set; }
        public string Username { get; set; }
        public string? URL { get; set; }
    }
}