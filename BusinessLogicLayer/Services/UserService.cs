using AutoMapper;
using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Entities;
using BusinessObjectLayer.Validators;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
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
        private readonly IValidator<UserEntity> userValidator;
        private readonly IValidator<AvailabilityEntity> availabilityValidator;
        private readonly IMapper mapper;

        public UserService(
            IUserRepository userRepository, 
            IDepartmentRepository departmentRepository, 
            IGroupRepository groupRepository, 
            IAvailabilityRepository availabilityRepository, 
            IValidator<UserEntity> userValidator,
            IValidator<AvailabilityEntity> availabilityValidator,
            IMapper mapper)
        {
            this.userRepository = userRepository;
            this.departmentRepository = departmentRepository;
            this.groupRepository = groupRepository;
            this.availabilityRepository = availabilityRepository;
            this.userValidator = userValidator;
            this.availabilityValidator = availabilityValidator;
            this.mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return (await userRepository.GetAllUsersAsync())
                .Select(mapper.Map<UserEntity, UserDto>)
                .ToList();
        }

        public async Task<List<UserDto>> GetUsersByLeaderGroupsAsync(string leaderUsername)
        {
            var leader = await userRepository.GetUserByUsernameAsync(leaderUsername);
            return (await userRepository.GetUsersByGroupIdAsync(leader.UserGroups.Select(x => x.GroupId).ToList()))
                .Select(mapper.Map<UserEntity, UserDto>)
                .ToList();
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

        public async Task<List<AvailabilityDto>> GetAvailabilitiesByUserUsernameAsync(string userUsername)
        {
            return (await availabilityRepository.GetAvailabilitiesByUserUsernameAsync(userUsername))
                 .Select(mapper.Map<AvailabilityEntity, AvailabilityDto>)
                 .ToList();
        }

        public async Task<AvailabilityDto> GetAvailabilityByFromDateAndUserUsernameAsync(DateTime fromDate, string userUsername)
        {
            var availability = await availabilityRepository.GetAvailabilityByFromDateAndUserUsernameAsync(fromDate, userUsername);
            return mapper.Map<AvailabilityEntity, AvailabilityDto>(availability);
        }

        public List<WeekDto> GetAllWeeksFromCurrentYear()
        {
            var jan1 = new DateTime(DateTime.Today.Year, 1, 1);

            var startOfFirstWeek = jan1.AddDays(1 - (int)(jan1.DayOfWeek));
            var weeks =
                Enumerable
                    .Range(0, 54)
                    .Select(i => new {
                        weekStart = startOfFirstWeek.AddDays(i * 7)
                    })
                    .TakeWhile(x => x.weekStart.Year <= jan1.Year)
                    .Select(x => new {
                        x.weekStart,
                        weekFinish = x.weekStart.AddDays(4)
                    })
                    .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
                    .Select((x, i) => new WeekDto{
                        FirstDay =  x.weekStart,
                        LastDay = x.weekFinish,
                        Number  = i + 1
                    });

            return weeks.ToList();
        }

        public async Task CreateUserAsync(UserDto user)
        {
            try
            {
                var userEntity = mapper.Map<UserDto, UserEntity>(user);
                userValidator.Validate(userEntity);
                await userRepository.CreateUserAsync(userEntity);
            }
            catch (ValidationException exception)
            {
                throw new ValidationException(exception.Message);
            }

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

            var weeks = GetAllWeeksFromCurrentYear();
            var availabilities = weeks.Select(x => new AvailabilityDto
            {
                UserUsername = user.Username,
                FromDate = x.FirstDay,
                ToDate = x.LastDay,
                AvailableHours = 20
            }).ToList();

            await CreateAvailabilityRangeAsync(availabilities);
        }

        public async Task CreateAvailabilityRangeAsync(List<AvailabilityDto> availabilities)
        {
            await availabilityRepository.CreateAvailabilityRangeAsync(availabilities.Select(mapper.Map<AvailabilityDto, AvailabilityEntity>).ToList());
        }

        public async Task UpdateAvailabilityAsync(AvailabilityDto availability)
        {
            try
            {
                var availabilityEntity = mapper.Map<AvailabilityDto, AvailabilityEntity>(availability);
                availabilityValidator.Validate(availabilityEntity);
                await availabilityRepository.UpdateAvailabilityAsync(availabilityEntity);
            }
            catch (ValidationException exception)
            {
                throw new ValidationException(exception.Message);
            }
        }
    }
}
