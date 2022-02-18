using BusinessObjectLayer.Entities;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public sealed class GroupRepository : IGroupRepository
    {
        private readonly EngiePlannerContext dbContext;

        public GroupRepository(EngiePlannerContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<List<GroupEntity>> GetAllGroupsAsync()
        {
            return dbContext.Groups.AsNoTracking().ToListAsync();
        }

        public Task<GroupEntity> GetGroupByNameAsync(string name)
        {
            return dbContext.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == name);
        }

        public Task<List<GroupEntity>> GetGroupsByUserUsernameAsync(string userUsername)
        {
            return dbContext.UserGroupMappings
                .Where(x => x.UserUsername == userUsername)
                .Select(x => x.Group)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CreateGroupAsync(GroupEntity group)
        {
            dbContext.Groups.Add(group);
            await dbContext.SaveChangesAsync();

            return group.Id;
        }

        public async Task CreateUserGroupMappingAsync(UserGroupMapping userGroupMapping)
        {
            dbContext.UserGroupMappings.Add(userGroupMapping);
            await dbContext.SaveChangesAsync();
        }
    }
}
