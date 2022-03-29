using BusinessObjectLayer.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserEntity>> GetAllUsersAsync();
        Task<List<UserEntity>> GetUsersByGroupIdAsync(ICollection<int> groupIds);
        Task<UserEntity> GetUserByUsernameAsync(string username);
        Task CreateUserAsync(UserEntity user);
    }
}
