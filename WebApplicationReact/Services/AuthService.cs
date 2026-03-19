using System.Security.Cryptography;
using System.Text;
using WebApplicationReact.Helpers;
using WebApplicationReact.Models.DTOs;
using WebApplicationReact.Models.Entities;
using WebApplicationReact.Models.Responses;
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

        public async Task<ApiResponse<object>> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _repository.GetByEmailAsync(request.Email);
            string? fileName = null;

            if (existingUser != null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "User already exists",
                    Data = null
                };
            }

            if (request.Image != null)
            {
                fileName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Image.CopyToAsync(stream);
                }
            }

            CreatePasswordHash(request.Password, out string hash, out string salt);

            var user = new User
            {
                UserName = request.Name,
                UserEmail = request.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                UserRole = request.Role ?? "User",
                ImageUrl = fileName != null ? "images/" + fileName : null
            };

            await _repository.AddAsync(user);

            return new ApiResponse<object>
            {
                Success = true,
                Message = "User registered successfully",
                Data = null
            };
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _repository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "Invalid credentials",
                    Data = null
                };
            }

            bool validPassword = VerifyPasswordHash(
                request.Password,
                user.PasswordHash,
                user.PasswordSalt
            );

            if (!validPassword)
            {
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "Invalid credentials",
                    Data = null
                };
            }

            var token = _jwt.GenerateToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

            await _repository.UpdateAsync(user);

            return new ApiResponse<AuthResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = new AuthResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Role = user.UserRole
                }
            };
        }

        public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string refreshToken)
        {
            var user = await _repository.GetByRefreshTokenAsync(refreshToken);

            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "Invalid or expired refresh token"
                };
            }

            var newAccessToken = _jwt.GenerateToken(user);

            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

            await _repository.UpdateAsync(user);

            return new ApiResponse<AuthResponse>
            {
                Success = true,
                Data = new AuthResponse
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    Role = user.UserRole
                }
            };
        }



        public async Task RevokeTokenAsync(string refreshToken)
        {
            var user = await _repository.GetByRefreshTokenAsync(refreshToken);

            if (user == null) return;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _repository.UpdateAsync(user);
        }

        private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using var hmac = new HMACSHA512();

            byte[] saltBytes = hmac.Key;
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            passwordSalt = Convert.ToBase64String(saltBytes);
            passwordHash = Convert.ToBase64String(hashBytes);
        }

        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            using var hmac = new HMACSHA512(saltBytes);

            byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            string computedHashString = Convert.ToBase64String(computedHash);

            return computedHashString == storedHash;
        }

        private string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
