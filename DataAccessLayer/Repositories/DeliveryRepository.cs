
using BusinessObjectLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly EngiePlannerContext dbContext;

        public DeliveryRepository(EngiePlannerContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<List<DeliveryEntity>> GetAllDeliveriesAsync()
        {
            return dbContext.Delivery.AsNoTracking().ToListAsync();
        }
    }
}
