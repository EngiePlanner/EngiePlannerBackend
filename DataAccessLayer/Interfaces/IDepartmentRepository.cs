using BusinessObjectLayer.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<List<DepartmentEntity>> GetAllDepartmentsAsync();
        Task<DepartmentEntity> GetDepartmentByNameAsync(string name);
        Task<List<DepartmentEntity>> GetDepartmentsByUserUsernameAsync(string userUsername);
        Task<int> CreateDepartmentAsync(DepartmentEntity department);
        Task CreateUserDepartmentMappingAsync(UserDepartmentMapping userDepartmentMapping);
    }
}
