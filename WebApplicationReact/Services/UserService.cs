using WebApplicationReact.Models.DTOs;
using WebApplicationReact.Models.Responses;
using WebApplicationReact.Repositories.Interfaces;
using WebApplicationReact.Services.Interfaces;

namespace WebApplicationReact.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<List<UserDetail>>> GetUsersAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
                pageNumber = 1;

            if (pageSize <= 0)
                pageSize = 10;

            // Prevent very large requests
            pageSize = Math.Min(pageSize, 50);

            var users = await _repository.GetUsersAsync(pageNumber, pageSize);

            if (users == null || users.Count == 0)
            {
                return new ApiResponse<List<UserDetail>>
                {
                    Success = false,
                    Message = "No users found",
                    Data = null
                };
            }

            return new ApiResponse<List<UserDetail>>
            {
                Success = true,
                Message = "Users retrieved successfully",
                Data = users
            };
        }
    }
}
