using AutoMapper;
using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Entities;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository taskRepository;
        private readonly IMapper mapper;

        public TaskService(ITaskRepository taskRepository, IMapper mapper)
        {
            this.taskRepository = taskRepository;
            this.mapper = mapper;
        }

        public async Task<List<TaskDto>> GetAllTasksAsync()
        {
            var tasks = (await taskRepository.GetAllTasksAsync())
                .Select(mapper.Map<TaskEntity, TaskDto>)
                .ToList();

            foreach (var task in tasks)
            {
                var employees = (await taskRepository.GetEmployeesByTaskIdAsync(task.Id))
                    .Select(mapper.Map<UserEntity, UserDto>)
                    .ToList();
                task.Employees = employees;
            }

            return tasks;
        }

        public async Task CreateTaskAsync(TaskDto task)
        {
            task.StartDate = DateTime.Now;
            var taskId = await taskRepository.CreateTaskAsync(mapper.Map<TaskDto, TaskEntity>(task));

            foreach (var employee in task.Employees)
            {
                var userTaskMapping = new UserTaskMapping
                {
                    UserUsername = employee.Username,
                    TaskId = taskId
                };

                await taskRepository.CreateUserTaskMappingAsync(userTaskMapping);
            }
        }
    }
}
