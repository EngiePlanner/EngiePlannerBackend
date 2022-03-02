using BusinessObjectLayer.Entities;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IAvailabilityRepository
    {
        Task CreateAvailabilityAsync(AvailabilityEntity availability);
    }
}
