using BusinessObjectLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskEntity>> GetAllTasksAsync();
        Task<List<TaskEntity>> GetTasksWithPlannedDateLowerThanGivenDateAsync(DateTime date);
        Task<List<TaskEntity>> GetPredecessorsByTaskIdAsync(int taskId);
        Task<List<TaskPredecessorMapping>> GetTaskPredecessorMappingsByTaskIdAsync(int taskId);
        Task<List<TaskPredecessorMapping>> GetTaskPredecessorMappingsByTaskOrPredecessorIdAsync(int id);
        Task<int> CreateTaskAsync(TaskEntity task);
        Task CreateTaskPredecessorMappingRangeAsync(List<TaskPredecessorMapping> taskPredecessorMappings);
        Task UpdateTaskAsync(TaskEntity task);
        Task UpdateTaskRangeAsync(List<TaskEntity> tasks);
        Task DeleteTaskAsync(int taskId);
        Task DeleteTaskPredecessorMappingRangeAsync(List<TaskPredecessorMapping> taskPredecessorMappings);
    }
}
