using BusinessObjectLayer.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task CreateUserAsync(UserDto user);
    }
}
