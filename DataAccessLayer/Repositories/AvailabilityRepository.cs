using BusinessObjectLayer.Entities;
using DataAccessLayer.Interfaces;
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

        public async Task CreateAvailabilityAsync(AvailabilityEntity availability)
        {
            dbContext.Availabilities.Add(availability);
            await dbContext.SaveChangesAsync();
        }
    }
}
