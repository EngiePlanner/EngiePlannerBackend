using BusinessObjectLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskEntity>> GetAllTasksAsync();
        Task<List<UserEntity>> GetEmployeesByTaskIdAsync(int taskId);
        Task<int> CreateTaskAsync(TaskEntity task);
        Task CreateUserTaskMappingAsync(UserTaskMapping userTaskMapping);
    }
}
