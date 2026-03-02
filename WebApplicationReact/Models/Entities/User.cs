namespace WebApplicationReact.Models.Entities
{
    public class User
    {
        
        public int UserId { get; set; }
        required
        public string UserName { get; set; }
        required
        public string UserEmail { get; set; }
        required
        public string PasswordHash { get; set; } 
        required
        public string PasswordSalt { get; set; }

        public string? UserRole { get; set; }
    }
}
