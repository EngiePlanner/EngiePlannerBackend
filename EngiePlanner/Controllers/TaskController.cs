using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngiePlanner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService taskService;

        public TaskController(ITaskService taskService)
        {
            this.taskService = taskService;
        }

        [HttpGet("GetAllTasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await taskService.GetAllTasksAsync();

            if (!tasks.Any())
            {
                return NoContent();
            }

            return Ok(tasks);
        }

        [HttpGet("GetTasksWithPlannedDateLowerThanGivenDate")]
        public async Task<IActionResult> GetTasksWithPlannedDateLowerThanGivenDateAsync(DateTime date)
        {
            var tasks = await taskService.GetTasksWithPlannedDateLowerThanGivenDateAsync(date);

            if (!tasks.Any())
            {
                return NoContent();
            }

            return Ok(tasks);
        }

        [HttpPost("CreateTask")]
        public async Task<IActionResult> CreateTask([FromBody] TaskDto task)
        {
            try
            {
                await taskService.CreateTaskAsync(task);
                return Ok();
            }
            catch (ValidationException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPut("AddPredecessors")]
        public async Task<IActionResult> CreateTaskPredecessorMappings([FromQuery] int taskId, [FromBody] List<int> predecessorsId)
        {
            await taskService.CreateTaskPredecessorMappingsRangeAsync(taskId, predecessorsId);
            return Ok();
        }

        [HttpPut("UpdateTask")]
        public async Task<IActionResult> UpdateTask([FromBody] TaskDto task)
        {
            try
            {
                await taskService.UpdateTaskAsync(task);
                return Ok();
            }
            catch (ValidationException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("DeleteTask")]
        public async Task<IActionResult> DeleteTask([FromQuery] int taskId)
        {
            await taskService.DeleteTaskAsync(taskId);
            return Ok();
        }
    }
}
