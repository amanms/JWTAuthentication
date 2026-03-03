namespace WebApplicationReact.Models.Entities
{
    public class User
    {
        [key]
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public required string UserEmail { get; set; }
        public required string PasswordHash { get; set; } 
        public required string PasswordSalt { get; set; }
        public string? UserRole { get; set; }
    }
}
