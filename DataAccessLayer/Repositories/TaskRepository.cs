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
            return dbContext.Tasks.Include(x => x.Employee).AsNoTracking().ToListAsync();
        }

        public Task<List<TaskEntity>> GetPredecessorsByTaskIdAsync(int taskId)
        {
            return dbContext.Tasks
                .Where(x => x.SuccessorId == taskId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CreateTaskAsync(TaskEntity task)
        {
            dbContext.Tasks.Add(task);
            await dbContext.SaveChangesAsync();

            return task.Id;
        }

        public async Task UpdateTaskAsync(TaskEntity task)
        {
            dbContext.Tasks.Update(task);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateTaskRangeAsync(List<TaskEntity> tasks)
        {
            dbContext.Tasks.UpdateRange(tasks);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            var task = new TaskEntity { Id = taskId };
            dbContext.Tasks.Remove(task);
            await dbContext.SaveChangesAsync();
        }
    }
}
