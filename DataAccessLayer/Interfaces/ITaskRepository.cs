using BusinessObjectLayer.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskEntity>> GetAllTasksAsync();
        Task<List<UserEntity>> GetEmployeesByTaskIdAsync(int taskId);
        Task<int> CreateTaskAsync(TaskEntity task);
        Task CreateUserTaskMappingAsync(UserTaskMapping userTaskMapping);
        Task UpdateTaskAsync(TaskEntity task);
        Task DeleteTaskAsync(int taskId);
    }
}
