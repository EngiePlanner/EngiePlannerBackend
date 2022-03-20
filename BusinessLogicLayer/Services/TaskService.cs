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
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository taskRepository;
        private readonly IValidator<TaskEntity> taskValidator;
        private readonly IMapper mapper;

        public TaskService(
            ITaskRepository taskRepository, 
            IValidator<TaskEntity> taskValidator, 
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
                task.Predecessors = predecessors;
            }

            return tasks;
        }

        public async Task CreateTaskAsync(TaskDto task)
        {
            int taskId;
            try
            {
                var taskEntity = mapper.Map<TaskDto, TaskEntity>(task);
                taskValidator.Validate(taskEntity);
                taskId = await taskRepository.CreateTaskAsync(taskEntity);
            }
            catch (ValidationException exception)
            {
                throw new ValidationException(exception.Message);
            }
        }

        public async Task UpdateTaskAsync(TaskDto task)
        {
            try
            {
                var taskEntity = mapper.Map<TaskDto, TaskEntity>(task);
                taskValidator.Validate(taskEntity);
                await taskRepository.UpdateTaskAsync(taskEntity);
            }
            catch (ValidationException exception)
            {
                throw new ValidationException(exception.Message);
            }
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            var predecessors = await taskRepository.GetPredecessorsByTaskIdAsync(taskId);
            predecessors.ForEach(x => x.SuccessorId = null);
            await taskRepository.UpdateTaskRangeAsync(predecessors);
            await taskRepository.DeleteTaskAsync(taskId);
        }
    }
}
