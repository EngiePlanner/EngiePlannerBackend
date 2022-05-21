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
    public class TaskServiceTests
    {
        private IMapper GetMapper()
        {
            var mapperConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new Mappers());
            });
            return mapperConfig.CreateMapper();
        }

        private UserEntity GetResponsible()
        {
            var userGroupMapping = new UserGroupMapping
            {
                UserUsername = "Responsible",
                GroupId = 1
            };

            var userDepartmentMapping = new UserDepartmentMapping
            {
                UserUsername = "Responsible",
                DepartmentId = 1
            };

            return new UserEntity
            {
                Username = "Responsible",
                Name = "Responsible",
                DisplayName = "Responsible",
                Email = "Responsible",
                RoleType = RoleType.Associate,
                LeaderUsername = "Responsible",
                UserGroups = new List<UserGroupMapping> { userGroupMapping },
                UserDepartments = new List<UserDepartmentMapping> { userDepartmentMapping },
            };
        }

        private UserEntity GetOwner()
        {
            var userGroupMapping = new UserGroupMapping
            {
                UserUsername = "Owner",
                GroupId = 1
            };

            var userDepartmentMapping = new UserDepartmentMapping
            {
                UserUsername = "Owner",
                DepartmentId = 1
            };

            return new UserEntity
            {
                Username = "Owner",
                Name = "Owner",
                DisplayName = "Owner",
                Email = "Owner",
                RoleType = RoleType.Associate,
                LeaderUsername = "Owner",
                UserGroups = new List<UserGroupMapping> { userGroupMapping },
                UserDepartments = new List<UserDepartmentMapping> { userDepartmentMapping },
            };
        }

        private List<TaskEntity> GetPredecessors()
        {
            var predecessor = new TaskEntity
            {
                Id = 2,
                Name = "Predecessor",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            return new List<TaskEntity> { predecessor };
        }

        private List<TaskEntity> GetUnscheduledPredecessors()
        {
            var predecessor = new TaskEntity
            {
                Id = 2,
                Name = "Predecessor",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
            };

            return new List<TaskEntity> { predecessor };
        }

        [Fact]
        public async void GetAllUsers_ExistsData_ResturnsList()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            var mapper = GetMapper();

            var predecessorDto = new TaskDto
            {
                Id = 2,
                Name = "Predecessor",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entity = new TaskEntity
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entities = new List<TaskEntity> { entity };
            var expectedNumberOfItems = 1;

            var expectedResult = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
                Predecessors = new List<TaskDto> { predecessorDto },
                ResponsibleUsername = "Responsible",
                ResponsibleDisplayName ="Responsible",
                OwnerUsername = "Owner"
            };

            taskRepository
                .Setup(x => x.GetAllTasksAsync())
                .ReturnsAsync(entities);
            taskRepository
                .Setup(x => x.GetPredecessorsByTaskIdAsync(1))
                .ReturnsAsync(GetPredecessors());
            taskRepository
                .Setup(x => x.GetUserByTaskIdAndUserTypeAsync(1, UserType.Responsible))
                .ReturnsAsync(GetResponsible());
            taskRepository
                .Setup(x => x.GetUserByTaskIdAndUserTypeAsync(1, UserType.Owner))
                .ReturnsAsync(GetOwner());

            var service = new TaskService(taskRepository.Object, null, mapper);

            //act
            var result = await service.GetAllTasksAsync();

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
            result.First().Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void GetTasksByOwnerUsername_ExistsData_ResturnsList()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            var mapper = GetMapper();

            var predecessorDto = new TaskDto
            {
                Id = 2,
                Name = "Predecessor",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entity = new TaskEntity
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entities = new List<TaskEntity> { entity };
            var expectedNumberOfItems = 1;

            var expectedResult = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
                Predecessors = new List<TaskDto> { predecessorDto },
                ResponsibleUsername = "Responsible",
                ResponsibleDisplayName = "Responsible",
                OwnerUsername = "Owner"
            };

            taskRepository
                .Setup(x => x.GetTasksByOwnerUsernameAsync("Owner"))
                .ReturnsAsync(entities);
            taskRepository
                .Setup(x => x.GetPredecessorsByTaskIdAsync(1))
                .ReturnsAsync(GetPredecessors());
            taskRepository
                .Setup(x => x.GetUserByTaskIdAndUserTypeAsync(1, UserType.Responsible))
                .ReturnsAsync(GetResponsible());

            var service = new TaskService(taskRepository.Object, null, mapper);

            //act
            var result = await service.GetTasksByOwnerUsernameAsync("Owner");

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
            result.First().Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void GetTasksWithPlannedDateLowerThanGivenDate_ExistsData_ResturnsList()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            var mapper = GetMapper();

            var entity = new TaskEntity
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entities = new List<TaskEntity> { entity };
            var expectedNumberOfItems = 1;

            var expectedResult = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            taskRepository
                .Setup(x => x.GetTasksWithPlannedDateLowerThanGivenDateAsync(DateTime.Parse("2022-05-27")))
                .ReturnsAsync(entities);

            var service = new TaskService(taskRepository.Object, null, mapper);

            //act
            var result = await service.GetTasksWithPlannedDateLowerThanGivenDateAsync(DateTime.Parse("2022-05-27"));

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
            result.First().Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void GetUnscheduledTasks_ExistsData_ResturnsList()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            var mapper = GetMapper();

            var predecessorDto = new TaskDto
            {
                Id = 2,
                Name = "Predecessor",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entity = new TaskEntity
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entities = new List<TaskEntity> { entity };
            var expectedNumberOfItems = 1;

            var expectedResult = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
                Predecessors = new List<TaskDto> { predecessorDto },
                ResponsibleUsername = "Responsible",
                ResponsibleDisplayName = "Responsible",
                OwnerUsername = "Owner"
            };

            taskRepository
                .Setup(x => x.GetUnscheduledTasksAsync())
                .ReturnsAsync(entities);
            taskRepository
                .Setup(x => x.GetPredecessorsByTaskIdAsync(1))
                .ReturnsAsync(GetPredecessors());
            taskRepository
                .Setup(x => x.GetUserByTaskIdAndUserTypeAsync(1, UserType.Responsible))
                .ReturnsAsync(GetResponsible());
            taskRepository
                .Setup(x => x.GetUserByTaskIdAndUserTypeAsync(1, UserType.Owner))
                .ReturnsAsync(GetOwner());

            var service = new TaskService(taskRepository.Object, null, mapper);

            //act
            var result = await service.GetUnscheduledTasksAsync();

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
            result.First().Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void GetUnscheduledTasksByOwnerUsername_ExistsData_ResturnsList()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            var mapper = GetMapper();

            var predecessorDto = new TaskDto
            {
                Id = 2,
                Name = "Predecessor",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
            };

            var entity = new TaskEntity
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
            };

            var entities = new List<TaskEntity> { entity };
            var expectedNumberOfItems = 1;

            var expectedResult = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                Predecessors = new List<TaskDto> { predecessorDto },
                ResponsibleUsername = "Responsible",
                ResponsibleDisplayName = "Responsible",
                OwnerUsername = "Owner"
            };

            taskRepository
                .Setup(x => x.GetTasksByOwnerUsernameAsync("Owner"))
                .ReturnsAsync(entities);
            taskRepository
                .Setup(x => x.GetPredecessorsByTaskIdAsync(1))
                .ReturnsAsync(GetUnscheduledPredecessors());
            taskRepository
                .Setup(x => x.GetUserByTaskIdAndUserTypeAsync(1, UserType.Responsible))
                .ReturnsAsync(GetResponsible());

            var service = new TaskService(taskRepository.Object, null, mapper);

            //act
            var result = await service.GetUnscheduledTasksByOwnerUsernameAsync("Owner");

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
            result.First().Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void GetTasksByOwnerUsernameWithPlannedDateLowerThanGivenDate_ExistsData_ResturnsList()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            var mapper = GetMapper();

            var entity = new TaskEntity
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entities = new List<TaskEntity> { entity };
            var expectedNumberOfItems = 1;

            var expectedResult = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            taskRepository
                .Setup(x => x.GetTasksByOwnerUsernameAsync("Owner"))
                .ReturnsAsync(entities);

            var service = new TaskService(taskRepository.Object, null, mapper);

            //act
            var result = await service.GetTasksByOwnerUsernameWithPlannedDateLowerThanGivenDateAsync("Owner", DateTime.Parse("2022-05-28"));

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
            result.First().Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void GetScheduledTasks_ExistsData_ResturnsList()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            var mapper = GetMapper();

            var predecessorDto = new TaskDto
            {
                Id = 2,
                Name = "Predecessor",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entity = new TaskEntity
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entities = new List<TaskEntity> { entity };
            var expectedNumberOfItems = 1;

            var expectedResult = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                Predecessors = new List<TaskDto> { predecessorDto },
                ResponsibleUsername = "Responsible",
                ResponsibleDisplayName = "Responsible",
                OwnerUsername = "Owner",
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            taskRepository
                .Setup(x => x.GetScheduledTasksAsync())
                .ReturnsAsync(entities);
            taskRepository
                .Setup(x => x.GetPredecessorsByTaskIdAsync(1))
                .ReturnsAsync(GetPredecessors());
            taskRepository
                .Setup(x => x.GetUserByTaskIdAndUserTypeAsync(1, UserType.Responsible))
                .ReturnsAsync(GetResponsible());
            taskRepository
               .Setup(x => x.GetUserByTaskIdAndUserTypeAsync(1, UserType.Owner))
               .ReturnsAsync(GetOwner());

            var service = new TaskService(taskRepository.Object, null, mapper);

            //act
            var result = await service.GetScheduledTasksAsync();

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
            result.First().Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void GetScheduledTasksByOwnerUsername_ExistsData_ResturnsList()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            var mapper = GetMapper();

            var predecessorDto = new TaskDto
            {
                Id = 2,
                Name = "Predecessor",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entity = new TaskEntity
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entities = new List<TaskEntity> { entity };
            var expectedNumberOfItems = 1;

            var expectedResult = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                Predecessors = new List<TaskDto> { predecessorDto },
                ResponsibleUsername = "Responsible",
                ResponsibleDisplayName = "Responsible",
                OwnerUsername = "Owner",
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            taskRepository
                .Setup(x => x.GetTasksByOwnerUsernameAsync("Owner"))
                .ReturnsAsync(entities);
            taskRepository
                .Setup(x => x.GetPredecessorsByTaskIdAsync(1))
                .ReturnsAsync(GetPredecessors());
            taskRepository
                .Setup(x => x.GetUserByTaskIdAndUserTypeAsync(1, UserType.Responsible))
                .ReturnsAsync(GetResponsible());
            

            var service = new TaskService(taskRepository.Object, null, mapper);

            //act
            var result = await service.GetScheduledTasksByOwnerUsernameAsync("Owner");

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
            result.First().Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void GetScheduledTasksByResponsibleUsername_ExistsData_ResturnsList()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            var mapper = GetMapper();

            var predecessorDto = new TaskDto
            {
                Id = 2,
                Name = "Predecessor",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entity = new TaskEntity
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            var entities = new List<TaskEntity> { entity };
            var expectedNumberOfItems = 1;

            var expectedResult = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                Predecessors = new List<TaskDto> { predecessorDto },
                ResponsibleUsername = "Responsible",
                ResponsibleDisplayName = "Responsible",
                OwnerUsername = "Owner",
                StartDate = DateTime.Parse("2022-05-18"),
                EndDate = DateTime.Parse("2022-05-19"),
            };

            taskRepository
                .Setup(x => x.GetTasksByResponsibleUsernameAsync("Responsible"))
                .ReturnsAsync(entities);
            taskRepository
                .Setup(x => x.GetPredecessorsByTaskIdAsync(1))
                .ReturnsAsync(GetPredecessors());
            taskRepository
                .Setup(x => x.GetUserByTaskIdAndUserTypeAsync(1, UserType.Responsible))
                .ReturnsAsync(GetResponsible());
            taskRepository
                .Setup(x => x.GetUserByTaskIdAndUserTypeAsync(1, UserType.Owner))
                .ReturnsAsync(GetOwner());


            var service = new TaskService(taskRepository.Object, null, mapper);

            //act
            var result = await service.GetScheduledTasksByResponsibleUsernameAsync("Responsible");

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
            result.First().Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void CreateTaskAsync_ValidTask_AddObjectToDb()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            IValidator<TaskDto> taskValidator = new TaskValidator();
            var mapper = GetMapper();

            var task = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                ResponsibleUsername = "Responsible",
                OwnerUsername = "Owner",
            };

            taskRepository
                .Setup(x => x.CreateTaskAsync(It.IsAny<TaskEntity>()))
                .ReturnsAsync(1);
            taskRepository
                .Setup(x => x.CreateUserTaskMappingAsync(It.IsAny<UserTaskMapping>()))
                .Returns(Task.CompletedTask);
            taskRepository
                .Setup(x => x.CreateUserTaskMappingAsync(It.IsAny<UserTaskMapping>()))
                .Returns(Task.CompletedTask);

            var service = new TaskService(taskRepository.Object, taskValidator, mapper);

            //act
            await service.CreateTaskAsync(task);

            //assert
            taskRepository.Verify(x => x.CreateTaskAsync(It.IsAny<TaskEntity>()), Times.Once);
            taskRepository.Verify(x => x.CreateUserTaskMappingAsync(It.IsAny<UserTaskMapping>()), Times.Exactly(2));
        }

        [Fact]
        public async void CreateTaskAsync_InvalidTask_AddObjectToDb()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            IValidator<TaskDto> taskValidator = new TaskValidator();
            var mapper = GetMapper();

            var task = new TaskDto
            {
                Id = 1,
                Name = "",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                ResponsibleUsername = "Responsible",
                OwnerUsername = "Owner",
            };

            taskRepository
                .Setup(x => x.CreateTaskAsync(It.IsAny<TaskEntity>()))
                .ReturnsAsync(1);
            taskRepository
                .Setup(x => x.CreateUserTaskMappingAsync(It.IsAny<UserTaskMapping>()))
                .Returns(Task.CompletedTask);
            taskRepository
                .Setup(x => x.CreateUserTaskMappingAsync(It.IsAny<UserTaskMapping>()))
                .Returns(Task.CompletedTask);

            var service = new TaskService(taskRepository.Object, taskValidator, mapper);

            //act
            var ex = await Assert.ThrowsAsync<ValidationException>(async () => await service.CreateTaskAsync(task));

            //assert
            Assert.Equal("Invalid name!\n", ex.Message);
            taskRepository.Verify(x => x.CreateTaskAsync(It.IsAny<TaskEntity>()), Times.Never);
        }

        [Fact]
        public async void CreateTaskPredecessorsMappingsRange_Valid_AddObjectToDb()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            var mapper = GetMapper();

            var task = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                ResponsibleUsername = "Responsible",
                OwnerUsername = "Owner",
            };

            var taskPredecessorMapping = new TaskPredecessorMapping
            {
                TaskId = 1,
                PredecessorId = 2
            };
            var entities = new List<TaskPredecessorMapping> { taskPredecessorMapping };

            taskRepository
                .Setup(x => x.GetTaskPredecessorMappingsByTaskIdAsync(1))
                .ReturnsAsync(entities);
            taskRepository
                .Setup(x => x.DeleteTaskPredecessorMappingRangeAsync(entities))
                .Returns(Task.CompletedTask);
            taskRepository
                .Setup(x => x.CreateTaskPredecessorMappingRangeAsync(It.IsAny<List<TaskPredecessorMapping>>()))
                .Returns(Task.CompletedTask);

            var service = new TaskService(taskRepository.Object, null, mapper);

            //act
            await service.CreateTaskPredecessorMappingsRangeAsync(1, new List<int> { 2 });

            //assert
            taskRepository.Verify(x => x.CreateTaskPredecessorMappingRangeAsync(It.IsAny<List<TaskPredecessorMapping>>()), Times.Once);
        }

        [Fact]
        public async void UpdateTaskAsync_ValidTask_UpdateObjectToDb()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            IValidator<TaskDto> taskValidator = new TaskValidator();
            var mapper = GetMapper();

            var task = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                ResponsibleUsername = "Responsible",
                OwnerUsername = "Owner",
            };

            var userTaskMapping = new UserTaskMapping
            {
                TaskId = 1,
                UserUsername = "Resp",
                UserType = UserType.Responsible
            };

            taskRepository
                .Setup(x => x.UpdateTaskAsync(It.IsAny<TaskEntity>()))
                .Returns(Task.CompletedTask);
            taskRepository
                .Setup(x => x.GetUserTaskMappingByTaskIdAndUserTypeAsync(1, UserType.Responsible))
                .ReturnsAsync(userTaskMapping);
            taskRepository
                .Setup(x => x.DeleteUserTaskMappingAsync(It.IsAny<UserTaskMapping>()))
                .Returns(Task.CompletedTask);
            taskRepository
                .Setup(x => x.CreateUserTaskMappingAsync(It.IsAny<UserTaskMapping>()))
                .Returns(Task.CompletedTask);

            var service = new TaskService(taskRepository.Object, taskValidator, mapper);

            //act
            await service.UpdateTaskAsync(task);

            //assert
            taskRepository.Verify(x => x.UpdateTaskAsync(It.IsAny<TaskEntity>()), Times.Once);
            taskRepository.Verify(x => x.CreateUserTaskMappingAsync(It.IsAny<UserTaskMapping>()), Times.Once);
        }

        [Fact]
        public async void UpdateTaskAsync_InvalidTask_UpdateObjectToDb()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            IValidator<TaskDto> taskValidator = new TaskValidator();
            var mapper = GetMapper();

            var task = new TaskDto
            {
                Id = 1,
                Name = "",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                ResponsibleUsername = "Responsible",
                OwnerUsername = "Owner",
            };

            var userTaskMapping = new UserTaskMapping
            {
                TaskId = 1,
                UserUsername = "Resp",
                UserType = UserType.Responsible
            };

            var service = new TaskService(taskRepository.Object, taskValidator, mapper);

            //act
            var ex = await Assert.ThrowsAsync<ValidationException>(async () => await service.UpdateTaskAsync(task));

            //assert
            Assert.Equal("Invalid name!\n", ex.Message);
            taskRepository.Verify(x => x.UpdateTaskAsync(It.IsAny<TaskEntity>()), Times.Never);
        }

        [Fact]
        public async void UpdateTaskRangeAsync_ValidTasks_UpdateObjectToDb()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            IValidator<TaskDto> taskValidator = new TaskValidator();
            var mapper = GetMapper();

            var task = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                ResponsibleUsername = "Responsible",
                OwnerUsername = "Owner",
            };
            var tasks = new List<TaskDto> { task };

            taskRepository
                .Setup(x => x.UpdateTaskRangeAsync(It.IsAny<List<TaskEntity>>()))
                .Returns(Task.CompletedTask);

            var service = new TaskService(taskRepository.Object, taskValidator, mapper);

            //act
            await service.UpdateTaskRangeAsync(tasks);

            //assert
            taskRepository.Verify(x => x.UpdateTaskRangeAsync(It.IsAny<List<TaskEntity>>()), Times.Once);
        }

        [Fact]
        public async void UpdateTaskRangeAsync_InvalidTasks_UpdateObjectToDb()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            IValidator<TaskDto> taskValidator = new TaskValidator();
            var mapper = GetMapper();

            var task = new TaskDto
            {
                Id = 1,
                Name = "",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                ResponsibleUsername = "Responsible",
                OwnerUsername = "Owner",
            };
            var tasks = new List<TaskDto> { task };

            var service = new TaskService(taskRepository.Object, taskValidator, mapper);

            //act
            var ex = await Assert.ThrowsAsync<ValidationException>(async () => await service.UpdateTaskRangeAsync(tasks));

            //assert
            Assert.Equal("Invalid name!\n", ex.Message);
            taskRepository.Verify(x => x.UpdateTaskRangeAsync(It.IsAny<List<TaskEntity>>()), Times.Never);
        }

        [Fact]
        public async void DeleteTaskRangeAsync_ExistingTask_RemoveObjectToDb()
        {
            //arrange
            var taskRepository = new Mock<ITaskRepository>();
            var mapper = GetMapper();

            var taskPredecessorMapping = new TaskPredecessorMapping
            {
                TaskId = 1,
                PredecessorId = 2
            };
            var taskPredecessorMappings = new List<TaskPredecessorMapping> { taskPredecessorMapping };

            taskRepository
                .Setup(x => x.GetTaskPredecessorMappingsByTaskOrPredecessorIdAsync(1))
                .ReturnsAsync(taskPredecessorMappings);
            taskRepository
                .Setup(x => x.DeleteTaskPredecessorMappingRangeAsync(taskPredecessorMappings))
                .Returns(Task.CompletedTask);
            taskRepository
                .Setup(x => x.DeleteTaskAsync(1))
                .Returns(Task.CompletedTask);

            var service = new TaskService(taskRepository.Object, null, mapper);

            //act
            await service.DeleteTaskAsync(1);

            //assert
            taskRepository.Verify(x => x.DeleteTaskAsync(It.IsAny<int>()), Times.Once);
        }
    }
}
