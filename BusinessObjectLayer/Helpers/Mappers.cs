using AutoMapper;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.Helpers
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<UserEntity, UserDto>()
                 .ForMember(x => x.Departments, opt => opt.Ignore())
                 .ForMember(x => x.Groups, opt => opt.Ignore());
            CreateMap<UserDto, UserEntity>();

            CreateMap<DepartmentEntity, DepartmentDto>();
            CreateMap<DepartmentDto, DepartmentEntity>();

            CreateMap<GroupEntity, GroupDto>();
            CreateMap<GroupDto, GroupEntity>();

            CreateMap<TaskEntity, TaskDto>()
                .ForMember(x => x.Employees, opt => opt.Ignore());
            CreateMap<TaskDto, TaskEntity>()
                .ForMember(x => x.Employees, opt => opt.Ignore());

            CreateMap<AvailabilityEntity, AvailabilityDto>();
            CreateMap<AvailabilityDto, AvailabilityEntity>()
                .ForMember(x => x.User, opt => opt.Ignore());
        }
    }
}
