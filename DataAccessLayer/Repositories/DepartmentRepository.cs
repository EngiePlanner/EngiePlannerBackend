using BusinessObjectLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public sealed class DepartmentRepository : IDepartmentRepository
    {
        private readonly EngiePlannerContext dbContext;

        public DepartmentRepository(EngiePlannerContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<List<DepartmentEntity>> GetAllDepartmentsAsync()
        {
            return dbContext.Departments.AsNoTracking().ToListAsync();
        }

        public Task<DepartmentEntity> GetDepartmentByNameAsync(string name)
        {
            return dbContext.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == name);
        }

        public Task<List<DepartmentEntity>> GetDepartmentsByUserUsernameAsync(string userUsername)
        {
            return dbContext.UserDepartmentMappings
                .Where(x => x.UserUsername == userUsername)
                .Select(x => x.Department)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CreateDepartmentAsync(DepartmentEntity department)
        {
            dbContext.Departments.Add(department);
            await dbContext.SaveChangesAsync();

            return department.Id;
        }

        public async Task CreateUserDepartmentMappingAsync(UserDepartmentMapping userDepartmentMapping)
        {
            dbContext.UserDepartmentMappings.Add(userDepartmentMapping);
            await dbContext.SaveChangesAsync();

        }
    }
}
