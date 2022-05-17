using BusinessObjectLayer.Entities;
using BusinessObjectLayer.Enums;
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
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<TaskEntity>> GetTasksByOwnerUsernameAsync(string ownerUsername)
        {
            return dbContext.UserTaskMappings
                .Where(x => x.UserUsername == ownerUsername && x.UserType == UserType.Owner)
                .Include(x => x.Task)
                .Select(x => x.Task)
                .ToListAsync();
        }

        public Task<List<TaskEntity>> GetTasksByResponsibleUsernameAsync(string responsibleUsername)
        {
            return dbContext.UserTaskMappings
                .Where(x => x.UserUsername == responsibleUsername && x.UserType == UserType.Responsible)
                .Include(x => x.Task)
                .Select(x => x.Task)
                .ToListAsync();
        }

        public Task<List<TaskEntity>> GetTasksWithPlannedDateLowerThanGivenDateAsync(DateTime date)
        {
            return dbContext.Tasks
                .Where(x => DateTime.Compare(x.PlannedDate, date) < 0)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<TaskEntity>> GetUnscheduledTasksAsync()
        {
            return dbContext.Tasks
                .Where(x => x.EndDate == null)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<TaskEntity>> GetScheduledTasksAsync()
        {
            return dbContext.Tasks
                .Where(x => x.EndDate != null)
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

        public Task<UserEntity> GetUserByTaskIdAndUserTypeAsync(int taskId, UserType userType)
        {
            return dbContext.UserTaskMappings
                .Where(x => x.TaskId == taskId && x.UserType == userType)
                .Include(x => x.User)
                .Select(x => x.User)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public Task<UserTaskMapping> GetUserTaskMappingByTaskIdAndUserTypeAsync(int taskId, UserType userType)
        {
            return dbContext.UserTaskMappings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TaskId == taskId && x.UserType == userType);
        }

        public async Task<int> CreateTaskAsync(TaskEntity task)
        {
            dbContext.Tasks.Add(task);
            await dbContext.SaveChangesAsync();

            return task.Id;
        }

        public async Task CreateTaskPredecessorMappingRangeAsync(List<TaskPredecessorMapping> taskPredecessorMappings)
        {
            dbContext.TaskPredecessorMappings.AddRange(taskPredecessorMappings);
            await dbContext.SaveChangesAsync();
        }

        public async Task CreateUserTaskMappingAsync(UserTaskMapping userTaskMapping)
        {
            dbContext.UserTaskMappings.Add(userTaskMapping);
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

        public async Task DeleteUserTaskMappingAsync(UserTaskMapping userTaskMapping)
        {
            dbContext.UserTaskMappings.Remove(userTaskMapping);
            await dbContext.SaveChangesAsync();
        }
    }
}
