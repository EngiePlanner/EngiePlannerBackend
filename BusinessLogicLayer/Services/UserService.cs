using AutoMapper;
using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Entities;
using DataAccessLayer.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IDepartmentRepository departmentRepository;
        private readonly IGroupRepository groupRepository;
        private readonly IAvailabilityRepository availabilityRepository;
        private readonly IMapper mapper;

        public UserService(IUserRepository userRepository, IDepartmentRepository departmentRepository, IGroupRepository groupRepository, IAvailabilityRepository availabilityRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.departmentRepository = departmentRepository;
            this.groupRepository = groupRepository;
            this.availabilityRepository = availabilityRepository;
            this.mapper = mapper;
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await userRepository.GetUserByUsernameAsync(username);
            var userDto = mapper.Map<UserEntity, UserDto>(user);

            if (user != null)
            {
                
                var groupLeader = await userRepository.GetUserByUsernameAsync(user.LeaderUsername);
                var departmentHead = await userRepository.GetUserByUsernameAsync(groupLeader.LeaderUsername);

                userDto.LeaderUsername = groupLeader.Username;
                userDto.LeaderName = groupLeader.Name;
                
                userDto.Departments = (await departmentRepository.GetDepartmentsByUserUsernameAsync(username)).Select(x => x.Name).ToList();
                userDto.Groups = (await groupRepository.GetGroupsByUserUsernameAsync(username)).Select(x => x.Name).ToList();
            }

            return userDto;
        }

        public async Task CreateUserAsync(UserDto user)
        {
            await userRepository.CreateUserAsync(mapper.Map<UserDto, UserEntity>(user));

            foreach (var departmentName in user.Departments)
            {
                var department = await departmentRepository.GetDepartmentByNameAsync(departmentName);
                if (department != null)
                {
                    var userDepartmentMapping = new UserDepartmentMapping
                    {
                        UserUsername = user.Username,
                        DepartmentId = department.Id
                    };
                    await departmentRepository.CreateUserDepartmentMappingAsync(userDepartmentMapping);
                }
                else
                {
                    var departmentDto = new DepartmentDto
                    {
                        Name = departmentName
                    };
                    var departmentId = await departmentRepository.CreateDepartmentAsync(mapper.Map<DepartmentDto, DepartmentEntity>(departmentDto));

                    var userDepartmentMapping = new UserDepartmentMapping
                    {
                        UserUsername = user.Username,
                        DepartmentId = departmentId
                    };
                    await departmentRepository.CreateUserDepartmentMappingAsync(userDepartmentMapping);
                }
            }

            foreach (var groupName in user.Groups)
            {
                var group = await groupRepository.GetGroupByNameAsync(groupName);
                if (group != null)
                {
                    var userGroupMapping = new UserGroupMapping
                    {
                        UserUsername = user.Username,
                        GroupId = group.Id
                    };
                    await groupRepository.CreateUserGroupMappingAsync(userGroupMapping);
                }
                else
                {
                    var department = await departmentRepository.GetDepartmentByNameAsync(user.Departments[0]);
                    var groupDto = new GroupDto
                    {
                        Name = groupName,
                        DepartmentId = department.Id
                    };
                    var groupId = await groupRepository.CreateGroupAsync(mapper.Map<GroupDto, GroupEntity>(groupDto));

                    var userGroupMapping = new UserGroupMapping
                    {
                        UserUsername = user.Username,
                        GroupId = groupId
                    };
                    await groupRepository.CreateUserGroupMappingAsync(userGroupMapping);
                }
            }
        }

        public async Task CreateAvailabilityAsync(AvailabilityDto availability)
        {
            availability.ToDate = availability.FromDate.AddDays(4);
            await availabilityRepository.CreateAvailabilityAsync(mapper.Map<AvailabilityDto, AvailabilityEntity>(availability));
        }
    }
}
