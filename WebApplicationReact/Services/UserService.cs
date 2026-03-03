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
    public class UserService:IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<UserDetail>> GetUsersAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}
