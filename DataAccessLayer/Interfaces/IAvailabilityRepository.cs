using BusinessObjectLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IAvailabilityRepository
    {
        Task<List<AvailabilityEntity>> GetAvailabilitiesByUserUsernameAsync(string userUsername);
        Task<AvailabilityEntity> GetAvailabilityByFromDateAndUserUsernameAsync(DateTime fromDate, string userUsername);
        Task CreateAvailabilityRangeAsync(List<AvailabilityEntity> availability);
        Task UpdateAvailabilityAsync(AvailabilityEntity availability);
    }
}
