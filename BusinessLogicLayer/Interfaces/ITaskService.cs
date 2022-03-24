using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetAllTasksAsync();
        Task<List<TaskDto>> GetTasksWithPlannedDateLowerThanGivenDateAsync(DateTime date);
        Task CreateTaskAsync(TaskDto task);
        Task CreateTaskPredecessorMappingsRangeAsync(int taskId, List<int> predecessorsId);
        Task UpdateTaskAsync(TaskDto task);
        Task DeleteTaskAsync(int taskId);
    }
}
