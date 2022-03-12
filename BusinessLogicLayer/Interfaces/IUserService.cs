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
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<List<AvailabilityDto>> GetAvailabilitiesByUserUsernameAsync(string userUsername);
        Task<AvailabilityDto> GetAvailabilityByFromDateAndUserUsernameAsync(DateTime fromDate, string userUsername);
        List<WeekDto> GetAllWeeksFromCurrentYear();
        Task CreateUserAsync(UserDto user);
        Task CreateAvailabilityRangeAsync(List<AvailabilityDto> availability);
        Task UpdateAvailabilityAsync(AvailabilityDto availability);
    }
}
