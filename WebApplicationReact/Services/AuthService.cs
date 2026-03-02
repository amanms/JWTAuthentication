using Azure.Core;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;
using WebApplicationReact.Helpers;
using WebApplicationReact.Models.DTOs;
using WebApplicationReact.Models.Entities;
using WebApplicationReact.Repositories.Interfaces;
using WebApplicationReact.Services.Interfaces;


namespace WebApplicationReact.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly JwtTokenGenerator _jwt;

        public AuthService(IUserRepository repository, JwtTokenGenerator jwt)
        {
            _repository = repository;
            _jwt = jwt;
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            var existingUser =
                await _repository.GetByEmailAsync(request.Email);

            if (existingUser != null)
                throw new Exception("User already exists");

            CreatePasswordHash(request.Password, out string hash, out string salt);

            var user = new User
            {
                UserName = request.Name,
                UserEmail = request.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                UserRole = request.Role ?? "User"
            };

            await _repository.AddAsync(user);
        }

        private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                byte[] saltBytes = hmac.Key; // automatically generated salt
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                passwordSalt = Convert.ToBase64String(saltBytes);
                passwordHash = Convert.ToBase64String(hashBytes);
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _repository.GetByEmailAsync(request.Email);

            if (user == null)
                throw new Exception("Invalid credentials");

            bool validPassword = VerifyPasswordHash(
                request.Password,
                user.PasswordHash,
                user.PasswordSalt
            );

            if (!validPassword)
                throw new Exception("Invalid credentials");

            var token = _jwt.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Role = user.UserRole
            };

        }

        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            using (var hmac = new HMACSHA512(saltBytes))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                string computedHashString = Convert.ToBase64String(computedHash);

                return computedHashString == storedHash;
            }
        }
    }

}
