using BusinessObjectLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly EngiePlannerContext dbContext;

        public AvailabilityRepository(EngiePlannerContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<List<AvailabilityEntity>> GetAvailabilitiesByUserUsernameAsync(string userUsername)
        {
            return dbContext.Availabilities
                .Where(x => x.UserUsername == userUsername)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<AvailabilityEntity> GetAvailabilityByFromDateAndUserUsernameAsync(DateTime fromDate, string userUsername)
        {
            return dbContext.Availabilities
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.FromDate == fromDate && x.UserUsername == userUsername);
        }

        public async Task CreateAvailabilityRangeAsync(List<AvailabilityEntity> availabilities)
        {
            dbContext.Availabilities.AddRange(availabilities);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateAvailabilityAsync(AvailabilityEntity availability)
        {
            dbContext.Availabilities.Update(availability);
            await dbContext.SaveChangesAsync();
        }
    }
}
