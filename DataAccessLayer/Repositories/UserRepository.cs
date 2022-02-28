using BusinessObjectLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public Task<UserEntity> GetUserByUsernameAsync(string username)
        {
            return dbContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task CreateUserAsync(UserEntity user)
        {
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }
    }
}
