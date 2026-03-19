namespace WebApplicationReact.Models.DTOs
{
    public class RegisterRequest
    {
        required
        public string Name { get; set; }
        required
        public string Email { get; set; }
        required
        public string Password { get; set; }
        public string? Role { get; set; }

        public IFormFile? Image { get; set; }
    }
}
