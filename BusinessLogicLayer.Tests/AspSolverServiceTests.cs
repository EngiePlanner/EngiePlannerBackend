using AutoMapper;
using BusinessLogicLayer.Services;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Entities;
using BusinessObjectLayer.Helpers;
using DataAccessLayer.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace BusinessLogicLayer.Tests
{
    public class AspSolverServiceTests
    {
        private IMapper GetMapper()
        {
            var mapperConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new Mappers());
            });
            return mapperConfig.CreateMapper();
        }

        [Fact]
        public async void InvokeAspSolver_ScheduleTasks_ResturnsList() {
            var taskRepository = new Mock<ITaskRepository>();
            var availabilityRepository = new Mock<IAvailabilityRepository>();
            var mapper = GetMapper();

            var task2 = new TaskDto
            {
                Id = 2,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-27"),
                Subteam = "Test",
                Duration = 10,
                Predecessors = new List<TaskDto>(),
                ResponsibleUsername = "Responsible",
                ResponsibleDisplayName = "Responsible",
                OwnerUsername = "Owner"
            };

            var task1 = new TaskDto
            {
                Id = 1,
                Name = "Test",
                AvailabilityDate = DateTime.Parse("2022-05-17"),
                PlannedDate = DateTime.Parse("2022-05-28"),
                Subteam = "Test",
                Duration = 10,
                Predecessors = new List<TaskDto> { task2 },
                ResponsibleUsername = "Responsible",
                ResponsibleDisplayName = "Responsible",
                OwnerUsername = "Owner"
            };

            var availability1 = new AvailabilityEntity
            {
                Id = 1,
                UserUsername = "Responsible",
                FromDate = DateTime.Parse("2022-05-16"),
                ToDate = DateTime.Parse("2022-05-20"),
                DefaultAvailableHours = 10,
                UnscheduledHours = 15
            };

            var availability2 = new AvailabilityEntity
            {
                Id = 1,
                UserUsername = "Responsible",
                FromDate = DateTime.Parse("2022-05-23"),
                ToDate = DateTime.Parse("2022-05-27"),
                DefaultAvailableHours = 20,
                UnscheduledHours = 20
            };

            var tasks = new List<TaskDto> { task1, task2 };
            var availabilities = new List<AvailabilityEntity> { availability1, availability2 };
            var expectedNumberOfItems = 2;

            availabilityRepository
                .Setup(x => x.GetAvailabilitiesByUserUsernameAsync("Responsible"))
                .ReturnsAsync(availabilities);

            var service = new AspSolverService(taskRepository.Object, availabilityRepository.Object, mapper);

            //act
            var result = await service.InvokeAspSolver(tasks);

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
            result.First().EndDate.Should().NotBeNull();
        }
    }
}
