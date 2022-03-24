using BusinessObjectLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
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
            return dbContext.Tasks
                .Include(x => x.Employee)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<TaskEntity>> GetTasksWithPlannedDateLowerThanGivenDateAsync(DateTime date)
        {
            return dbContext.Tasks
                .Where(x => DateTime.Compare(x.PlannedDate, date) < 0)
                .Include(x => x.Employee)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<TaskEntity>> GetPredecessorsByTaskIdAsync(int taskId)
        {
            return dbContext.TaskPredecessorMappings
                .Where(x => x.TaskId == taskId)
                .Select(x => x.Predecessor)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<TaskPredecessorMapping>> GetTaskPredecessorMappingsByTaskOrPredecessorIdAsync(int id)
        {
            return dbContext.TaskPredecessorMappings
                .Where(x => x.TaskId == id || x.PredecessorId == id)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<TaskPredecessorMapping>> GetTaskPredecessorMappingsByTaskIdAsync(int taskId)
        {
            return dbContext.TaskPredecessorMappings
                .Where(x => x.TaskId == taskId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CreateTaskAsync(TaskEntity task)
        {
            dbContext.Tasks.Add(task);
            await dbContext.SaveChangesAsync();

            return task.Id;
        }

        public async Task CreateTaskPredecessorMappingRangeAsync(List<TaskPredecessorMapping> taskPredecessorMappings)
        {
            dbContext.AddRange(taskPredecessorMappings);
            await dbContext.SaveChangesAsync();
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

        public async Task DeleteTaskPredecessorMappingRangeAsync(List<TaskPredecessorMapping> taskPredecessorMappings)
        {
            dbContext.TaskPredecessorMappings.RemoveRange(taskPredecessorMappings);
            await dbContext.SaveChangesAsync();
        }
    }
}
