using BusinessObjectLayer.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskEntity>> GetAllTasksAsync();
        Task<List<TaskEntity>> GetPredecessorsByTaskIdAsync(int taskId);
        Task<int> CreateTaskAsync(TaskEntity task);
        Task UpdateTaskAsync(TaskEntity task);
        Task UpdateTaskRangeAsync(List<TaskEntity> tasks);
        Task DeleteTaskAsync(int taskId);
    }
}
