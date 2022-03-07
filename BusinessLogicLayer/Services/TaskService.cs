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
        private readonly IUserRepository userRepository;
        private readonly IDeliveryRepository deliveryRepository;
        private readonly IMapper mapper;

        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository, IDeliveryRepository deliveryRepository, IMapper mapper)
        {
            this.taskRepository = taskRepository;
            this.userRepository = userRepository;
            this.deliveryRepository = deliveryRepository;
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
                    .Select(x => x.Username)
                    .ToList();
                task.Employees = employees;
            }

            return tasks;
        }

        public async Task<List<DeliveryDto>> GetAllDeliveriesAsync()
        {
            return (await deliveryRepository.GetAllDeliveriesAsync())
                .Select(mapper.Map<DeliveryEntity, DeliveryDto>)
                .ToList();
        }

        public async Task CreateTaskAsync(TaskDto task)
        {
            task.StartDate = DateTime.Now;
            var taskId = await taskRepository.CreateTaskAsync(mapper.Map<TaskDto, TaskEntity>(task));

            foreach (var employeeUsername in task.Employees)
            {
                var employee = await userRepository.GetUserByUsernameAsync(employeeUsername);
                var userTaskMapping = new UserTaskMapping
                {
                    UserUsername = employee.Username,
                    TaskId = taskId
                };

                await taskRepository.CreateUserTaskMappingAsync(userTaskMapping);
            }
        }

        public async Task UpdateTaskAsync(TaskDto task)
        {
            await taskRepository.UpdateTaskAsync(mapper.Map<TaskDto, TaskEntity>(task));
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            await taskRepository.DeleteTaskAsync(taskId);
        }
    }
}
