using BusinessObjectLayer.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IGroupRepository
    {
        Task<List<GroupEntity>> GetAllGroupsAsync();
        Task<GroupEntity> GetGroupByNameAsync(string name);
        Task<List<GroupEntity>> GetGroupsByUserUsernameAsync(string unserUsername);
        Task<int> CreateGroupAsync(GroupEntity group);
        Task CreateUserGroupMappingAsync(UserGroupMapping userGroupMapping);
    }
}
