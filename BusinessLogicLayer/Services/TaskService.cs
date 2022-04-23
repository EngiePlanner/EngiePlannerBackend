using AutoMapper;
using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Entities;
using BusinessObjectLayer.Enums;
using BusinessObjectLayer.Validators;
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
        private readonly IValidator<TaskDto> taskValidator;
        private readonly IMapper mapper;

        public TaskService(
            ITaskRepository taskRepository, 
            IValidator<TaskDto> taskValidator, 
            IMapper mapper)
        {
            this.taskRepository = taskRepository;
            this.taskValidator = taskValidator;
            this.mapper = mapper;
        }

        public async Task<List<TaskDto>> GetAllTasksAsync()
        {
            var tasks = (await taskRepository.GetAllTasksAsync())
                .Select(mapper.Map<TaskEntity, TaskDto>)
                .ToList();

            foreach (var task in tasks)
            {
                var predecessors = (await taskRepository.GetPredecessorsByTaskIdAsync(task.Id))
                    .Select(mapper.Map<TaskEntity, TaskDto>)
                    .ToList();
                var responsibleUsername = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Responsible)).Username;
                var ownerUsername = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Owner)).Username;
                var responsibleDisplayName = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Responsible)).DisplayName;

                task.Predecessors = predecessors;
                task.ResponsibleUsername = responsibleUsername;
                task.OwnerUsername = ownerUsername;
                task.ResponsibleDisplayName = responsibleDisplayName;
            }

            return tasks;
        }

        public async Task<List<TaskDto>> GetTasksByOwnerUsername(string ownerUsername)
        {
            var tasks = (await taskRepository.GetTasksByOwnerUsername(ownerUsername))
                .Select(mapper.Map<TaskEntity, TaskDto>)
                .ToList();

            foreach (var task in tasks)
            {
                var predecessors = (await taskRepository.GetPredecessorsByTaskIdAsync(task.Id))
                    .Select(mapper.Map<TaskEntity, TaskDto>)
                    .ToList();
                var responsibleUsername = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Responsible)).Username;
                var responsibleDisplayName = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Responsible)).DisplayName;

                task.Predecessors = predecessors;
                task.ResponsibleUsername = responsibleUsername;
                task.OwnerUsername = ownerUsername;
                task.ResponsibleDisplayName = responsibleDisplayName;
            }

            return tasks;
        }

        public async Task<List<TaskDto>> GetTasksWithPlannedDateLowerThanGivenDateAsync(DateTime date)
        {
            return (await taskRepository.GetTasksWithPlannedDateLowerThanGivenDateAsync(date))
                .Select(mapper.Map<TaskEntity, TaskDto>)
                .ToList();
        }

        public async Task<List<TaskDto>> GetUnscheduledTasksAsync()
        {
            var tasks = (await taskRepository.GetUnscheduledTasksAsync())
                .Select(mapper.Map<TaskEntity, TaskDto>)
                .ToList();

            foreach (var task in tasks)
            {
                var predecessors = (await taskRepository.GetPredecessorsByTaskIdAsync(task.Id))
                    .Select(mapper.Map<TaskEntity, TaskDto>)
                    .ToList();
                var responsibleUsername = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Responsible)).Username;
                var responsibleDisplayName = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Responsible)).DisplayName;
                var ownerUsername = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Owner)).Username;

                task.Predecessors = predecessors;
                task.ResponsibleUsername = responsibleUsername;
                task.ResponsibleDisplayName = responsibleDisplayName;
                task.OwnerUsername = ownerUsername;
            }

            return tasks;
        }

        public async Task<List<TaskDto>> GetUnscheduledTasksByOwnerUsernameAsync(string ownerUsername)
        {
            var tasks = (await taskRepository.GetTasksByOwnerUsername(ownerUsername))
               .Where(x => x.EndDate == null)
               .Select(mapper.Map<TaskEntity, TaskDto>)
               .ToList();

            foreach (var task in tasks)
            {
                var predecessors = (await taskRepository.GetPredecessorsByTaskIdAsync(task.Id))
                  .Select(mapper.Map<TaskEntity, TaskDto>)
                  .ToList();
                var responsibleUsername = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Responsible)).Username;
                var responsibleDisplayName = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Responsible)).DisplayName;

                task.Predecessors = predecessors;
                task.ResponsibleUsername = responsibleUsername;
                task.ResponsibleDisplayName = responsibleDisplayName;
                task.OwnerUsername = ownerUsername;
            }

            return tasks;
        }

        public async Task<List<TaskDto>> GetTasksByOwnerUsernameWithPlannedDateLowerThanGivenDateAsync(string ownerUsername, DateTime date)
        {
            return (await taskRepository.GetTasksByOwnerUsername(ownerUsername))
                .Where(x => DateTime.Compare(x.PlannedDate, date) < 0).ToList()
                .Select(mapper.Map<TaskEntity, TaskDto>)
                .ToList();
        }

        public async Task<List<TaskDto>> GetScheduledTasksAsync()
        {
            var tasks = (await taskRepository.GetScheduledTasksAsync())
                .Select(mapper.Map<TaskEntity, TaskDto>)
                .ToList();

            foreach (var task in tasks)
            {
                var predecessors = (await taskRepository.GetPredecessorsByTaskIdAsync(task.Id))
                    .Select(mapper.Map<TaskEntity, TaskDto>)
                    .ToList();
                var responsibleUsername = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Responsible)).Username;
                var responsibleDisplayName = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Responsible)).DisplayName;
                var ownerUsername = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Owner)).Username;

                task.Predecessors = predecessors;
                task.ResponsibleUsername = responsibleUsername;
                task.ResponsibleDisplayName = responsibleDisplayName;
                task.OwnerUsername = ownerUsername;
            }

            return tasks;
        }
        
        public async Task<List<TaskDto>> GetScheduledTasksByOwnerUsernameAsync(string ownerUsername)
        {
            var tasks = (await taskRepository.GetTasksByOwnerUsername(ownerUsername))
               .Where(x => x.EndDate != null)
               .Select(mapper.Map<TaskEntity, TaskDto>)
               .ToList();

            foreach (var task in tasks)
            {
                var predecessors = (await taskRepository.GetPredecessorsByTaskIdAsync(task.Id))
                  .Select(mapper.Map<TaskEntity, TaskDto>)
                  .ToList();
                var responsibleUsername = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Responsible)).Username;
                var responsibleDisplayName = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Responsible)).DisplayName;

                task.Predecessors = predecessors;
                task.ResponsibleUsername = responsibleUsername;
                task.ResponsibleDisplayName = responsibleDisplayName;
                task.OwnerUsername = ownerUsername;
            }

            return tasks;
        }
        
        public async Task<List<TaskDto>> GetScheduledTasksByResponsibleUsernameAsync(string responsibleUsername)
        {
            var tasks = (await taskRepository.GetTasksByResponsibleUsername(responsibleUsername))
               .Where(x => x.EndDate != null)
               .Select(mapper.Map<TaskEntity, TaskDto>)
               .ToList();

            foreach (var task in tasks)
            {
                var predecessors = (await taskRepository.GetPredecessorsByTaskIdAsync(task.Id))
                  .Select(mapper.Map<TaskEntity, TaskDto>)
                  .ToList();
                var responsibleDisplayName = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Responsible)).DisplayName;
                var ownerUsername = (await taskRepository.GetUserByTaskIdAndUserType(task.Id, UserType.Owner)).Username;

                task.Predecessors = predecessors;
                task.ResponsibleUsername = responsibleUsername;
                task.ResponsibleDisplayName = responsibleDisplayName;
                task.OwnerUsername = ownerUsername;
            }

            return tasks;
        }

        public async Task CreateTaskAsync(TaskDto task)
        {
            int taskId;
            try
            {
                taskValidator.Validate(task);
                var taskEntity = mapper.Map<TaskDto, TaskEntity>(task);
                taskId = await taskRepository.CreateTaskAsync(taskEntity);

                var ownerTaskMapping = new UserTaskMapping
                {
                    UserUsername = task.OwnerUsername,
                    TaskId = taskId,
                    UserType = UserType.Owner
                };
                await taskRepository.CreateUserTaskMappingAsync(ownerTaskMapping);

                var responsibleTaskMapping = new UserTaskMapping
                {
                    UserUsername = task.ResponsibleUsername,
                    TaskId = taskId,
                    UserType = UserType.Responsible
                };
                await taskRepository.CreateUserTaskMappingAsync(responsibleTaskMapping);
            }
            catch (ValidationException exception)
            {
                throw new ValidationException(exception.Message);
            }
        }

        public async Task CreateTaskPredecessorMappingsRangeAsync(int taskId, List<int> predecessorsId)
        {
            var oldTaskPredecessorMappings = await taskRepository.GetTaskPredecessorMappingsByTaskIdAsync(taskId);
            await taskRepository.DeleteTaskPredecessorMappingRangeAsync(oldTaskPredecessorMappings);
            
            var taskPredecessorMappings = new List<TaskPredecessorMapping>();
            foreach (var predecessorId in predecessorsId)
            {
                var taskPredecessorMapping = new TaskPredecessorMapping
                {
                    TaskId = taskId,
                    PredecessorId = predecessorId
                };

                taskPredecessorMappings.Add(taskPredecessorMapping);
            }

            await taskRepository.CreateTaskPredecessorMappingRangeAsync(taskPredecessorMappings);
        }

        public async Task UpdateTaskAsync(TaskDto task)
        {
            try
            {
                taskValidator.Validate(task);
                var taskEntity = mapper.Map<TaskDto, TaskEntity>(task);
                await taskRepository.UpdateTaskAsync(taskEntity);
                var userTaskMapping = await taskRepository.GetUserTaskMappingByTaskIdAndUserType(task.Id, UserType.Responsible);
                if (task.ResponsibleUsername != userTaskMapping.UserUsername)
                {
                    await taskRepository.DeleteUserTaskMappingAsync(userTaskMapping);
                    userTaskMapping = new UserTaskMapping
                    {
                        UserUsername = task.ResponsibleUsername,
                        TaskId = task.Id,
                        UserType = UserType.Responsible
                    };
                    await taskRepository.CreateUserTaskMappingAsync(userTaskMapping);
                }
            }
            catch (ValidationException exception)
            {
                throw new ValidationException(exception.Message);
            }
        }

        public async Task UpdateTaskRangeAsync(List<TaskDto> tasks)
        {
            try
            {
                foreach (var task in tasks)
                {
                    taskValidator.Validate(task);
                }

                await taskRepository.UpdateTaskRangeAsync(tasks.Select(mapper.Map<TaskDto, TaskEntity>).ToList());
            }
            catch (ValidationException exception)
            {
                throw new ValidationException(exception.Message);
            }
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            var taskPredecessorMapping = await taskRepository.GetTaskPredecessorMappingsByTaskOrPredecessorIdAsync(taskId);
            await taskRepository.DeleteTaskPredecessorMappingRangeAsync(taskPredecessorMapping);
            await taskRepository.DeleteTaskAsync(taskId);
        }
    }
}
