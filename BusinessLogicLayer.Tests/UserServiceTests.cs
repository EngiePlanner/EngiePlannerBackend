using AutoMapper;
using BusinessLogicLayer.Services;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Entities;
using BusinessObjectLayer.Enums;
using BusinessObjectLayer.Helpers;
using BusinessObjectLayer.Validators;
using DataAccessLayer.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BusinessLogicLayer.Tests
{
    public class UserServiceTests
    {
        private IMapper GetMapper()
        {
            var mapperConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new Mappers());
            });
            return mapperConfig.CreateMapper();
        }

        [Fact]
        public async void GetAllUsers_ExistsData_ResturnsList()
        {
            //arrange
            var userRepository = new Mock<IUserRepository>();
            var mapper = GetMapper();
            var entity = new UserEntity
            {
                Username = "Test",
                Name = "Test",
                DisplayName = "Test",
                Email = "Test",
                RoleType = RoleType.Admin,
                LeaderUsername = "Test"
            };

            var entities = new List<UserEntity> { entity };
            var expectedNumberOfItems = 1;

            var expectedResult = new UserDto
            {
                Username = "Test",
                Name = "Test",
                DisplayName = "Test",
                Email = "Test",
                RoleType = RoleType.Admin,
                LeaderUsername = "Test"
            };

            userRepository
                .Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(entities);

            var service = new UserService(userRepository.Object, null, null, null, null, null, mapper);

            //act
            var result = await service.GetAllUsersAsync();

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
            result.First().Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void GetUsersByLeaderGroupsAsync_ExistsData_ResturnsList()
        {
            //arrange
            var userRepository = new Mock<IUserRepository>();
            var mapper = GetMapper();

            var userGroupMapping = new UserGroupMapping
            {
                UserUsername = "Test",
                GroupId = 1
            };

            var entity = new UserEntity
            {
                Username = "Test",
                Name = "Test",
                DisplayName = "Test",
                Email = "Test",
                RoleType = RoleType.Admin,
                LeaderUsername = "Test",
                UserGroups = new List<UserGroupMapping> { userGroupMapping }
            };

            var entities = new List<UserEntity> { entity };
            var expectedNumberOfItems = 1;

            var expectedResult = new UserDto
            {
                Username = "Test",
                Name = "Test",
                DisplayName = "Test",
                Email = "Test",
                RoleType = RoleType.Admin,
                LeaderUsername = "Test",
            };

            userRepository
                .Setup(x => x.GetUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(entity);

            userRepository
                .Setup(x => x.GetUsersByGroupIdAsync(It.IsAny<ICollection<int>>()))
                .ReturnsAsync(entities);

            var service = new UserService(userRepository.Object, null, null, null, null, null, mapper);

            //act
            var result = await service.GetUsersByLeaderGroupsAsync(It.IsAny<string>());

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
            result.First().Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void GetUserByUsername_ExistsData_ResturnsObject()
        {
            //arrange
            var userRepository = new Mock<IUserRepository>();
            var groupRepository = new Mock<IGroupRepository>();
            var departmentRepository = new Mock<IDepartmentRepository>();
            var mapper = GetMapper();

            var userGroupMapping = new UserGroupMapping
            {
                UserUsername = "User",
                GroupId = 1
            };

            var userDepartmentMapping = new UserDepartmentMapping
            {
                UserUsername = "User",
                DepartmentId = 1
            };

            var user = new UserEntity
            {
                Username = "User",
                Name = "User",
                DisplayName = "User",
                Email = "User",
                RoleType = RoleType.Associate,
                LeaderUsername = "GroupLeader",
                UserGroups = new List<UserGroupMapping> { userGroupMapping },
                UserDepartments = new List<UserDepartmentMapping> { userDepartmentMapping },
            };

            var groupLeader = new UserEntity
            {
                Username = "GroupLeader",
                Name = "GroupLeader",
                DisplayName = "GroupLeader",
                Email = "GroupLeader",
                RoleType = RoleType.Leader,
                LeaderUsername = "DepartmentHead",
                UserGroups = new List<UserGroupMapping> { userGroupMapping },
                UserDepartments = new List<UserDepartmentMapping> { userDepartmentMapping }
            };

            var group = new GroupEntity
            {
                Id = 1,
                Name = "Group",
                DepartmentId = 1
            };
            var groups = new List<GroupEntity> { group };

            var department = new DepartmentEntity
            {
                Id = 1,
                Name = "Department"
            };
            var departments = new List<DepartmentEntity> { department };

            var expectedResult = new UserDto
            {
                Username = "User",
                Name = "User",
                DisplayName = "User",
                Email = "User",
                RoleType = RoleType.Associate,
                LeaderUsername = "GroupLeader",
                LeaderName = "GroupLeader",
                Departments = new List<string> { "Department" },
                Groups = new List<string> { "Group"}
            };

            userRepository
                .Setup(x => x.GetUserByUsernameAsync("User"))
                .ReturnsAsync(user);
            userRepository
               .Setup(x => x.GetUserByUsernameAsync("GroupLeader"))
               .ReturnsAsync(groupLeader);

            departmentRepository
                .Setup(x => x.GetDepartmentsByUserUsernameAsync("User"))
                .ReturnsAsync(departments);

            groupRepository
                .Setup(x => x.GetGroupsByUserUsernameAsync("User"))
                .ReturnsAsync(groups);

            var service = new UserService(userRepository.Object, departmentRepository.Object, groupRepository.Object, null, null, null, mapper);

            //act
            var result = await service.GetUserByUsernameAsync("User");

            //assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void GetAvailabilitiesByUserUsername_ExistsData_ResturnsList()
        {
            //arrange
            var availabilityRepository = new Mock<IAvailabilityRepository>();
            var mapper = GetMapper();

            var availability = new AvailabilityEntity
            {
                Id = 1,
                UserUsername = "User",
                FromDate = DateTime.Parse("2022-05-17"),
                ToDate = DateTime.Parse("2022-05-24"),
                DefaultAvailableHours = 20,
                UnscheduledHours = 15
            };

            var expectedResult = new AvailabilityDto
            {
                Id = 1,
                UserUsername = "User",
                FromDate = DateTime.Parse("2022-05-17"),
                ToDate = DateTime.Parse("2022-05-24"),
                DefaultAvailableHours = 20,
                UnscheduledHours = 15
            };
            var expectedNumberOfItems = 1;

            availabilityRepository
                .Setup(x => x.GetAvailabilitiesByUserUsernameAsync("User"))
                .ReturnsAsync(new List<AvailabilityEntity> { availability });
           

            var service = new UserService(null, null, null, availabilityRepository.Object, null, null, mapper);

            //act
            var result = await service.GetAvailabilitiesByUserUsernameAsync("User");

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
            result.First().Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void GetAvailabilityByFromDateAndUserUsername_ExistsData_ResturnsObject()
        {
            //arrange
            var availabilityRepository = new Mock<IAvailabilityRepository>();
            var mapper = GetMapper();

            var availability = new AvailabilityEntity
            {
                Id = 1,
                UserUsername = "User",
                FromDate = DateTime.Parse("2022-05-17"),
                ToDate = DateTime.Parse("2022-05-24"),
                DefaultAvailableHours = 20,
                UnscheduledHours = 15
            };

            var expectedResult = new AvailabilityDto
            {
                Id = 1,
                UserUsername = "User",
                FromDate = DateTime.Parse("2022-05-17"),
                ToDate = DateTime.Parse("2022-05-24"),
                DefaultAvailableHours = 20,
                UnscheduledHours = 15
            };

            availabilityRepository
                .Setup(x => x.GetAvailabilityByFromDateAndUserUsernameAsync(DateTime.Parse("2022-05-17"), "User"))
                .ReturnsAsync(availability);


            var service = new UserService(null, null, null, availabilityRepository.Object, null, null, mapper);

            //act
            var result = await service.GetAvailabilityByFromDateAndUserUsernameAsync(DateTime.Parse("2022-05-17"), "User");

            //assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetAllWeeksFromCurrentYear_ExistsData_ResturnsList()
        {
            //arrange

            var expectedResult = new WeekDto
            {
                FirstDay = DateTime.Parse("2022-01-03"),
                LastDay = DateTime.Parse("2022-01-07"),
                Number = 1
            };
            var nrOfWeeks = (DateTime.IsLeapYear(2022) ? 366 : 365) / 7;

            var service = new UserService(null, null, null, null, null, null, null);

            //act
            var result = service.GetAllWeeksFromCurrentYear();

            //assert
            Assert.Equal(result.Count, nrOfWeeks);
            result.First().Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void CreateUserAsync_ValidUser_AddObjectToDb()
        {
            //arrange
            var userRepository = new Mock<IUserRepository>();
            var departmentRepository = new Mock<IDepartmentRepository>();
            var groupRepository = new Mock<IGroupRepository>();
            var availabilityRepository = new Mock<IAvailabilityRepository>();
            IValidator<UserEntity> userValidator = new UserValidator();
            IValidator<AvailabilityEntity> availabilityValidator = new AvailabilityValidator();
            var mapper = GetMapper();

            var user = new UserDto
            {
               Username = "Test",
               Name = "Test",
               DisplayName = "Test",
               Email = "Test",
               RoleType = RoleType.Admin,
               Departments = new List<string> { "D1", "D2" }, 
               Groups = new List<string> { "G1", "G2" },
               LeaderUsername = "Test",
               LeaderName = "Test"
            };

            var department = new DepartmentEntity
            {
                Id = 1,
                Name = "D1"
            };

            var group = new GroupEntity
            {
                Id = 1,
                Name = "G1",
                DepartmentId = 1
            };

            userRepository
                .Setup(x => x.CreateUserAsync(It.IsAny<UserEntity>()))
                .Returns(Task.CompletedTask);

            departmentRepository
                .Setup(x => x.GetDepartmentByNameAsync("D1"))
                .ReturnsAsync(department);
            departmentRepository
                .Setup(x => x.CreateUserDepartmentMappingAsync(It.IsAny<UserDepartmentMapping>()))
                .Returns(Task.CompletedTask);
            departmentRepository
                .Setup(x => x.CreateUserDepartmentMappingAsync(It.IsAny<UserDepartmentMapping>()))
                .Returns(Task.CompletedTask);
            departmentRepository
                .Setup(x => x.CreateDepartmentAsync(It.IsAny<DepartmentEntity>()))
                .ReturnsAsync(1);

            groupRepository
                .Setup(x => x.GetGroupByNameAsync("G1"))
                .ReturnsAsync(group);
            groupRepository
                .Setup(x => x.CreateUserGroupMappingAsync(It.IsAny<UserGroupMapping>()))
                .Returns(Task.CompletedTask);
            groupRepository
                .Setup(x => x.CreateUserGroupMappingAsync(It.IsAny<UserGroupMapping>()))
                .Returns(Task.CompletedTask);
            groupRepository
                .Setup(x => x.CreateGroupAsync(It.IsAny<GroupEntity>()))
                .ReturnsAsync(1);

            availabilityRepository
                .Setup(x => x.CreateAvailabilityRangeAsync(It.IsAny<List<AvailabilityEntity>>()))
                .Returns(Task.CompletedTask);

            var service = new UserService(userRepository.Object, departmentRepository.Object, groupRepository.Object, availabilityRepository.Object, userValidator, availabilityValidator, mapper);

            //act
            await service.CreateUserAsync(user);

            //assert
            userRepository.Verify(x => x.CreateUserAsync(It.IsAny<UserEntity>()), Times.Once);
        }

        [Fact]
        public async void CreateUserAsync_InvalidUser_ThrowsValidationException()
        {
            //arrange
            var userRepository = new Mock<IUserRepository>();
            var departmentRepository = new Mock<IDepartmentRepository>();
            var groupRepository = new Mock<IGroupRepository>();
            IValidator<UserEntity> userValidator = new UserValidator();
            var mapper = GetMapper();

            var user = new UserDto
            {
                Username = "",
                Name = "Test",
                DisplayName = "Test",
                Email = "Test",
                RoleType = RoleType.Admin,
                Departments = new List<string> { "D1", "D2" },
                Groups = new List<string> { "G1", "G2" },
                LeaderUsername = "Test",
                LeaderName = "Test"
            };

            userRepository
                .Setup(x => x.CreateUserAsync(It.IsAny<UserEntity>()))
                .Returns(Task.CompletedTask);

            var service = new UserService(userRepository.Object, departmentRepository.Object, groupRepository.Object, null, userValidator, null, mapper);

            //act
            var ex = await Assert.ThrowsAsync<ValidationException>(async () => await service.CreateUserAsync(user));

            //assert
            Assert.Equal("Invalid username!\n", ex.Message);
            userRepository.Verify(x => x.CreateUserAsync(It.IsAny<UserEntity>()), Times.Never);
        }


        [Fact]
        public async void CreateAvailabilityRangeAsync_ValidAvailabilities_AddObjectToDb()
        {
            //arrange
            var availabilityRepository = new Mock<IAvailabilityRepository>();
            var availabilityValidator = new Mock<IValidator<AvailabilityEntity>>();
            var mapper = GetMapper();

            var availabilityDto = new AvailabilityDto
            {
                Id = 1,
                UserUsername = "User",
                FromDate = DateTime.Parse("2022-05-17"),
                ToDate = DateTime.Parse("2022-05-24"),
                DefaultAvailableHours = 20,
                UnscheduledHours = 15
            };

            availabilityRepository
                .Setup(x => x.CreateAvailabilityRangeAsync(It.IsAny<List<AvailabilityEntity>>()))
                .Returns(Task.CompletedTask);

            var service = new UserService(null, null, null, availabilityRepository.Object, null, availabilityValidator.Object, mapper);

            //act
            await service.CreateAvailabilityRangeAsync(new List<AvailabilityDto> { availabilityDto });

            //assert
            availabilityRepository.Verify(x => x.CreateAvailabilityRangeAsync(It.IsAny<List<AvailabilityEntity>>()), Times.Once);
        }

        [Fact]
        public async void CreateAvailabilityRangeAsync_InvalidValidAvailabilities_ThrowValidationException()
        {
            //arrange
            var availabilityRepository = new Mock<IAvailabilityRepository>();
            IValidator<AvailabilityEntity> availabilityValidator = new AvailabilityValidator();
            var mapper = GetMapper();

            var availabilityDto = new AvailabilityDto
            {
                Id = 1,
                UserUsername = "",
                FromDate = DateTime.Parse("2022-05-16"),
                ToDate = DateTime.Parse("2022-05-20"),
                DefaultAvailableHours = 10,
                UnscheduledHours = 15
            };

            availabilityRepository
                .Setup(x => x.CreateAvailabilityRangeAsync(It.IsAny<List<AvailabilityEntity>>()))
                .Returns(Task.CompletedTask);

            var service = new UserService(null, null, null, availabilityRepository.Object, null, availabilityValidator, mapper);

            //act
            var ex = await Assert.ThrowsAsync<ValidationException>(async () => await service.CreateAvailabilityRangeAsync(new List<AvailabilityDto> { availabilityDto }));

            //assert
            Assert.Equal("Invalid username!\n", ex.Message);
            availabilityRepository.Verify(x => x.CreateAvailabilityRangeAsync(It.IsAny<List<AvailabilityEntity>>()), Times.Never);
        }

        [Fact]
        public async void UpdateDefaultAvailabileHoursAsync_ValidAvailability_UpdateAvailability()
        {
            //arrange
            var availabilityRepository = new Mock<IAvailabilityRepository>();
            IValidator<AvailabilityEntity> availabilityValidator = new AvailabilityValidator();
            var mapper = GetMapper();

            var newAvailability = new AvailabilityDto
            {
                Id = 1,
                UserUsername = "Test",
                FromDate = DateTime.Parse("2022-05-16"),
                ToDate = DateTime.Parse("2022-05-20"),
                DefaultAvailableHours = 13,
                UnscheduledHours = 10
            };

            var oldAvailability = new AvailabilityEntity
            {
                Id = 1,
                UserUsername = "Test",
                FromDate = DateTime.Parse("2022-05-16"),
                ToDate = DateTime.Parse("2022-05-20"),
                DefaultAvailableHours = 10,
                UnscheduledHours = 15
            };

            availabilityRepository
                .Setup(x => x.GetAvailabilityByIdAsync(1))
                .ReturnsAsync(oldAvailability);

            availabilityRepository
               .Setup(x => x.UpdateAvailabilityAsync(It.IsAny<AvailabilityEntity>()))
               .Returns(Task.CompletedTask);

            var service = new UserService(null, null, null, availabilityRepository.Object, null, availabilityValidator, mapper);

            //act
            await service.UpdateDefaultAvailabileHoursAsync(newAvailability);

            //assert
            availabilityRepository.Verify(x => x.UpdateAvailabilityAsync(It.IsAny<AvailabilityEntity>()), Times.Once);
        }

        [Fact]
        public async void UpdateDefaultAvailabileHoursAsync_ValidAvailabilityButNotEqualHours_UpdateAvailability()
        {
            //arrange
            var availabilityRepository = new Mock<IAvailabilityRepository>();
            IValidator<AvailabilityEntity> availabilityValidator = new AvailabilityValidator();
            var mapper = GetMapper();

            var newAvailability = new AvailabilityDto
            {
                Id = 1,
                UserUsername = "Test",
                FromDate = DateTime.Parse("2022-05-16"),
                ToDate = DateTime.Parse("2022-05-20"),
                DefaultAvailableHours = 13,
                UnscheduledHours = 10
            };

            var oldAvailability = new AvailabilityEntity
            {
                Id = 1,
                UserUsername = "Test",
                FromDate = DateTime.Parse("2022-05-16"),
                ToDate = DateTime.Parse("2022-05-20"),
                DefaultAvailableHours = 11,
                UnscheduledHours = 10
            };

            availabilityRepository
                .Setup(x => x.GetAvailabilityByIdAsync(1))
                .ReturnsAsync(oldAvailability);

            availabilityRepository
               .Setup(x => x.UpdateAvailabilityAsync(It.IsAny<AvailabilityEntity>()))
               .Returns(Task.CompletedTask);

            var service = new UserService(null, null, null, availabilityRepository.Object, null, availabilityValidator, mapper);

            //act
            await service.UpdateDefaultAvailabileHoursAsync(newAvailability);

            //assert
            availabilityRepository.Verify(x => x.UpdateAvailabilityAsync(It.IsAny<AvailabilityEntity>()), Times.Once);
        }

        [Fact]
        public async void UpdateDefaultAvailabileHoursAsync_InvalidAvailabilities_ThrowValidationException()
        {
            //arrange
            var availabilityRepository = new Mock<IAvailabilityRepository>();
            IValidator<AvailabilityEntity> availabilityValidator = new AvailabilityValidator();
            var mapper = GetMapper();

            var newAvailability = new AvailabilityDto
            {
                Id = 1,
                UserUsername = "",
                FromDate = DateTime.Parse("2022-05-16"),
                ToDate = DateTime.Parse("2022-05-20"),
                DefaultAvailableHours = 10,
                UnscheduledHours = 15
            };

            var oldAvailability = new AvailabilityEntity
            {
                Id = 1,
                UserUsername = "Test",
                FromDate = DateTime.Parse("2022-05-16"),
                ToDate = DateTime.Parse("2022-05-20"),
                DefaultAvailableHours = 10,
                UnscheduledHours = 15
            };

            availabilityRepository
                 .Setup(x => x.GetAvailabilityByIdAsync(1))
                 .ReturnsAsync(oldAvailability);

            availabilityRepository
               .Setup(x => x.UpdateAvailabilityAsync(It.IsAny<AvailabilityEntity>()))
               .Returns(Task.CompletedTask);

            var service = new UserService(null, null, null, availabilityRepository.Object, null, availabilityValidator, mapper);

            //act
            var ex = await Assert.ThrowsAsync<ValidationException>(async () => await service.UpdateDefaultAvailabileHoursAsync( newAvailability ));

            //assert
            Assert.Equal("Invalid username!\n", ex.Message);
            availabilityRepository.Verify(x => x.UpdateAvailabilityAsync(It.IsAny<AvailabilityEntity>()), Times.Never);
        }

        [Fact]
        public async void UpdateDefaultAvailabileHoursAsync_ValidAvailabilityButNotEnoughHours_ThrowsValidationException()
        {
            //arrange
            var availabilityRepository = new Mock<IAvailabilityRepository>();
            IValidator<AvailabilityEntity> availabilityValidator = new AvailabilityValidator();
            var mapper = GetMapper();

            var newAvailability = new AvailabilityDto
            {
                Id = 1,
                UserUsername = "Test",
                FromDate = DateTime.Parse("2022-05-16"),
                ToDate = DateTime.Parse("2022-05-20"),
                DefaultAvailableHours = 13,
                UnscheduledHours = 10
            };

            var oldAvailability = new AvailabilityEntity
            {
                Id = 1,
                UserUsername = "Test",
                FromDate = DateTime.Parse("2022-05-16"),
                ToDate = DateTime.Parse("2022-05-20"),
                DefaultAvailableHours = 14,
                UnscheduledHours = 14
            };

            availabilityRepository
                .Setup(x => x.GetAvailabilityByIdAsync(1))
                .ReturnsAsync(oldAvailability);

            availabilityRepository
               .Setup(x => x.UpdateAvailabilityAsync(It.IsAny<AvailabilityEntity>()))
               .Returns(Task.CompletedTask);

            var service = new UserService(null, null, null, availabilityRepository.Object, null, availabilityValidator, mapper);

            //act
            var ex = await Assert.ThrowsAsync<ValidationException>(async () => await service.UpdateDefaultAvailabileHoursAsync(newAvailability));

            //assert
            Assert.Equal("You already have more than 13 scheduled hours this week!", ex.Message);
            availabilityRepository.Verify(x => x.UpdateAvailabilityAsync(It.IsAny<AvailabilityEntity>()), Times.Never);
        }

        [Fact]
        public async void UpdateUnscheduledHoursAsync_ValidAvailability_UpdateAvailability()
        {
            //arrange
            var availabilityRepository = new Mock<IAvailabilityRepository>();

            var task1 = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-10"),
                PlannedDate = DateTime.Parse("2022-05-28"),
                Subteam = "Test",
                Duration = 13,
                Predecessors = new List<TaskDto>(),
                ResponsibleUsername = "Test",
                ResponsibleDisplayName = "Test",
                OwnerUsername = "Test",
                StartDate = DateTime.Parse("2022-05-16"),
                EndDate = DateTime.Parse("2022-05-18"),
            };

            var task2 = new TaskDto
            {
                Id = 2,
                Name = "Test2",
                AvailabilityDate = DateTime.Parse("2022-05-10"),
                PlannedDate = DateTime.Parse("2022-05-28"),
                Subteam = "Test2",
                Duration = 10,
                Predecessors = new List<TaskDto>(),
                ResponsibleUsername = "Test",
                ResponsibleDisplayName = "Test2",
                OwnerUsername = "Test2",
                StartDate = DateTime.Parse("2022-05-17"),
                EndDate = DateTime.Parse("2022-05-18"),
            };

            var tasks = new List<TaskDto> { task1, task2 };

            var availability = new AvailabilityEntity
            {
                Id = 1,
                UserUsername = "User",
                FromDate = DateTime.Parse("2022-05-16"),
                ToDate = DateTime.Parse("2022-05-20"),
                DefaultAvailableHours = 24,
                UnscheduledHours = 24
            };

            availabilityRepository
                .Setup(x => x.GetAvailabilityByFromDateAndUserUsernameAsync(DateTime.Parse("2022-05-16"), "Test"))
                .ReturnsAsync(availability);

            availabilityRepository
               .Setup(x => x.GetAvailabilityByFromDateAndUserUsernameAsync(DateTime.Parse("2022-05-16"), "Test"))
               .ReturnsAsync(availability);

            var service = new UserService(null, null, null, availabilityRepository.Object, null, null, null);

            //act
            await service.UpdateUnscheduledHoursAsync(tasks);

            //assert
            availabilityRepository.Verify(x => x.UpdateAvailabilityAsync(It.IsAny<AvailabilityEntity>()), Times.Once);
        }

        [Fact]
        public async void UpdateUnscheduledHoursAsync_NotEnoughHours_ThrowException()
        {
            //arrange
            var availabilityRepository = new Mock<IAvailabilityRepository>();

            var task1 = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-10"),
                PlannedDate = DateTime.Parse("2022-05-28"),
                Subteam = "Test",
                Duration = 13,
                Predecessors = new List<TaskDto>(),
                ResponsibleUsername = "Test",
                ResponsibleDisplayName = "Test",
                OwnerUsername = "Test",
                StartDate = DateTime.Parse("2022-05-16"),
                EndDate = DateTime.Parse("2022-05-18"),
            };

            var task2 = new TaskDto
            {
                Id = 2,
                Name = "Test2",
                AvailabilityDate = DateTime.Parse("2022-05-10"),
                PlannedDate = DateTime.Parse("2022-05-28"),
                Subteam = "Test2",
                Duration = 10,
                Predecessors = new List<TaskDto>(),
                ResponsibleUsername = "Test",
                ResponsibleDisplayName = "Test2",
                OwnerUsername = "Test2",
                StartDate = DateTime.Parse("2022-05-17"),
                EndDate = DateTime.Parse("2022-05-18"),
            };

            var tasks = new List<TaskDto> { task1, task2 };

            var availability = new AvailabilityEntity
            {
                Id = 1,
                UserUsername = "User",
                FromDate = DateTime.Parse("2022-05-16"),
                ToDate = DateTime.Parse("2022-05-20"),
                DefaultAvailableHours = 5,
                UnscheduledHours = 5
            };

            availabilityRepository
                .Setup(x => x.GetAvailabilityByFromDateAndUserUsernameAsync(DateTime.Parse("2022-05-16"), "Test"))
                .ReturnsAsync(availability);

            availabilityRepository
               .Setup(x => x.GetAvailabilityByFromDateAndUserUsernameAsync(DateTime.Parse("2022-05-16"), "Test"))
               .ReturnsAsync(availability);

            var service = new UserService(null, null, null, availabilityRepository.Object, null, null, null);

            //act
            var ex = await Assert.ThrowsAsync<ValidationException>(async () => await service.UpdateUnscheduledHoursAsync(tasks));

            //assert
            Assert.Equal(availability.UserUsername + " doesn't have enough available hours during the week " + availability.FromDate.ToShortDateString() + "-" + availability.ToDate.ToShortDateString(), ex.Message);
            availabilityRepository.Verify(x => x.UpdateAvailabilityAsync(It.IsAny<AvailabilityEntity>()), Times.Never);
        }
    }
}
