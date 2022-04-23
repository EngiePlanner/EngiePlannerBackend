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

        [HttpGet("GetTasksByOwnerUsername")]
        public async Task<IActionResult> GetTasksByOwnerUsername([FromQuery] string ownerUsername)
        {
            var tasks = await taskService.GetTasksByOwnerUsername(ownerUsername);

            if (!tasks.Any())
            {
                return NoContent();
            }

            return Ok(tasks);
        }

        [HttpGet("GetTasksWithPlannedDateLowerThanGivenDate")]
        public async Task<IActionResult> GetTasksWithPlannedDateLowerThanGivenDateAsync([FromQuery] DateTime date)
        {
            var tasks = await taskService.GetTasksWithPlannedDateLowerThanGivenDateAsync(date);

            if (!tasks.Any())
            {
                return NoContent();
            }

            return Ok(tasks);
        }

        [HttpGet("GetTasksByOwnerUsernameWithPlannedDateLowerThanGivenDate")]
        public async Task<IActionResult> GetTasksByOwnerUsernameWithPlannedDateLowerThanGivenDate([FromQuery] string ownerUsername, [FromQuery] DateTime date)
        {
            var tasks = await taskService.GetTasksByOwnerUsernameWithPlannedDateLowerThanGivenDateAsync(ownerUsername, date);

            if (!tasks.Any())
            {
                return NoContent();
            }

            return Ok(tasks);
        }

        [HttpGet("GetUnscheduledTasks")]
        public async Task<IActionResult> GetUnscheduledTasks()
        {
            var tasks = await taskService.GetUnscheduledTasksAsync();

            if (!tasks.Any())
            {
                return NoContent();
            }

            return Ok(tasks);
        }

        [HttpGet("GetUnscheduledTasksByOwnerUsername")]
        public async Task<IActionResult> GetUnscheduledTasksByOwnerUsername([FromQuery] string ownerUsername)
        {
            var tasks = await taskService.GetUnscheduledTasksByOwnerUsernameAsync(ownerUsername);

            if (!tasks.Any())
            {
                return NoContent();
            }

            return Ok(tasks);
        }

        [HttpGet("GetScheduledTasks")]
        public async Task<IActionResult> GetScheduledTasks()
        {
            var tasks = await taskService.GetScheduledTasksAsync();

            if (!tasks.Any())
            {
                return NoContent();
            }

            return Ok(tasks);
        }

        [HttpGet("GetScheduledTasksByOwnerUsername")]
        public async Task<IActionResult> GetScheduledTasksByOwnerUsername([FromQuery] string ownerUsername)
        {
            var tasks = await taskService.GetScheduledTasksByOwnerUsernameAsync(ownerUsername);

            if (!tasks.Any())
            {
                return NoContent();
            }

            return Ok(tasks);
        }

        [HttpGet("GetScheduledTasksByResponsibleUsername")]
        public async Task<IActionResult> GetScheduledTasksByResponsibleUsername([FromQuery] string responsibleUsername)
        {
            var tasks = await taskService.GetScheduledTasksByResponsibleUsernameAsync(responsibleUsername);

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

        [HttpPut("UpdateTasks")]
        public async Task<IActionResult> UpdateTasks([FromBody] List<TaskDto> tasks)
        {
            try
            {
                await taskService.UpdateTaskRangeAsync(tasks);
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
