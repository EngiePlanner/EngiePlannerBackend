using BusinessObjectLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public sealed class TaskRepository : ITaskRepository
    {
        private readonly EngiePlannerContext dbContext;

        public TaskRepository(EngiePlannerContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<List<TaskEntity>> GetAllTasksAsync()
        {
            return dbContext.Tasks.AsNoTracking().ToListAsync();
        }

        public Task<List<UserEntity>> GetEmployeesByTaskIdAsync(int taskId)
        {
            return dbContext.UserTaskMappings
                .Where(x => x.TaskId == taskId)
                .Select(x => x.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CreateTaskAsync(TaskEntity task)
        {
            dbContext.Tasks.Add(task);
            await dbContext.SaveChangesAsync();

            return task.Id;
        }

        public async Task CreateUserTaskMappingAsync(UserTaskMapping userTaskMapping)
        {
            dbContext.UserTaskMappings.Add(userTaskMapping);
            await dbContext.SaveChangesAsync();
        }
    }
}
