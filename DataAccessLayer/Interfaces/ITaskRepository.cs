using BusinessObjectLayer.Entities;
using BusinessObjectLayer.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskEntity>> GetAllTasksAsync();
        Task<List<TaskEntity>> GetTasksByOwnerUsername(string ownerUsername);
        Task<List<TaskEntity>> GetTasksByResponsibleUsername(string responsibleUsername);
        Task<List<TaskEntity>> GetTasksWithPlannedDateLowerThanGivenDateAsync(DateTime date);
        Task<List<TaskEntity>> GetUnscheduledTasksAsync();
        Task<List<TaskEntity>> GetScheduledTasksAsync();
        Task<List<TaskEntity>> GetPredecessorsByTaskIdAsync(int taskId);
        Task<List<TaskPredecessorMapping>> GetTaskPredecessorMappingsByTaskIdAsync(int taskId);
        Task<List<TaskPredecessorMapping>> GetTaskPredecessorMappingsByTaskOrPredecessorIdAsync(int id);
        Task<UserEntity> GetUserByTaskIdAndUserType(int taskId, UserType userType);
        Task<UserTaskMapping> GetUserTaskMappingByTaskIdAndUserType(int taskId, UserType userType);
        Task<int> CreateTaskAsync(TaskEntity task);
        Task CreateTaskPredecessorMappingRangeAsync(List<TaskPredecessorMapping> taskPredecessorMappings);
        Task CreateUserTaskMappingAsync(UserTaskMapping userTaskMapping);
        Task UpdateTaskAsync(TaskEntity task);
        Task UpdateTaskRangeAsync(List<TaskEntity> tasks);
        Task DeleteTaskAsync(int taskId);
        Task DeleteTaskPredecessorMappingRangeAsync(List<TaskPredecessorMapping> taskPredecessorMappings);
        Task DeleteUserTaskMappingAsync(UserTaskMapping userTaskMapping);
    }
}
