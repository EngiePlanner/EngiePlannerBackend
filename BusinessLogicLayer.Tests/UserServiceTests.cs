using AutoMapper;
using BusinessLogicLayer.Services;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Entities;
using BusinessObjectLayer.Enums;
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
        public async void GetAllUsers_NoData_ResturnsEmptyList()
        {
            //arrange
            var userRepository = new Mock<IUserRepository>();
            var mapper = GetMapper();
        
            var entities = new List<UserEntity>();
            var expectedNumberOfItems = 0;

            userRepository
                .Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(entities);

            var service = new UserService(userRepository.Object, null, null, null, null, null, mapper);

            //act
            var result = await service.GetAllUsersAsync();

            //assert
            Assert.Equal(result.Count, expectedNumberOfItems);
        }
    }
}
