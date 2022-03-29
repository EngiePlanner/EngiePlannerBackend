using BusinessObjectLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly EngiePlannerContext dbContext;

        public UserRepository(EngiePlannerContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<List<UserEntity>> GetAllUsersAsync()
        {
            return dbContext.Users.AsNoTracking().ToListAsync();
        }

        public Task<List<UserEntity>> GetUsersByGroupIdAsync(ICollection<int> groupIds)
        {
            return dbContext.Users
                .Where(x => x.UserGroups.Any(y => groupIds.Contains(y.GroupId)))
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<UserEntity> GetUserByUsernameAsync(string username)
        {
            return dbContext.Users.AsNoTracking()
                .Include(x => x.UserGroups)
                .FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task CreateUserAsync(UserEntity user)
        {
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }
    }
}
