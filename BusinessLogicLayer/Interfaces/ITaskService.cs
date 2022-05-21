using BusinessObjectLayer.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetAllTasksAsync();
        Task<List<TaskDto>> GetTasksByOwnerUsernameAsync(string ownerUsername);
        Task<List<TaskDto>> GetTasksWithPlannedDateLowerThanGivenDateAsync(DateTime date);
        Task<List<TaskDto>> GetTasksByOwnerUsernameWithPlannedDateLowerThanGivenDateAsync(string ownerUsername, DateTime date);
        Task<List<TaskDto>> GetUnscheduledTasksAsync();
        Task<List<TaskDto>> GetUnscheduledTasksByOwnerUsernameAsync(string ownerUsername);
        Task<List<TaskDto>> GetScheduledTasksAsync();
        Task<List<TaskDto>> GetScheduledTasksByOwnerUsernameAsync(string ownerUsername);
        Task<List<TaskDto>> GetScheduledTasksByResponsibleUsernameAsync(string responsibleUsername);
        Task CreateTaskAsync(TaskDto task);
        Task CreateTaskPredecessorMappingsRangeAsync(int taskId, List<int> predecessorsId);
        Task UpdateTaskAsync(TaskDto task);
        Task UpdateTaskRangeAsync(List<TaskDto> tasks);
        Task DeleteTaskAsync(int taskId);
    }
}
