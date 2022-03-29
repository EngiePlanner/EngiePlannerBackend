using BusinessObjectLayer.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetAllTasksAsync();
        Task<List<TaskDto>> GetTasksByOwnerUsername(string ownerUsername);
        Task<List<TaskDto>> GetTasksWithPlannedDateLowerThanGivenDateAsync(DateTime date);
        Task<List<TaskDto>> GetTasksByOwnerUsernameWithPlannedDateLowerThanGivenDateAsync(string ownerUsername, DateTime date);
        Task<List<TaskDto>> GetUnplannedTasksAsync();
        Task<List<TaskDto>> GetUnplannedTasksByOwnerUsernameAsync(string ownerUsername);
        Task CreateTaskAsync(TaskDto task);
        Task CreateTaskPredecessorMappingsRangeAsync(int taskId, List<int> predecessorsId);
        Task UpdateTaskAsync(TaskDto task);
        Task DeleteTaskAsync(int taskId);
    }
}
