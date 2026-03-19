namespace WebApplicationReact.Models.DTOs
{
    public class AuthResponse
    {
        public string? Token { get; set; }
        public string? Role { get; set; }
        public string? RefreshToken { get; set; }
    }
}
