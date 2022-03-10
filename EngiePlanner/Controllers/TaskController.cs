using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("GetAllDeliveries")]
        public async Task<IActionResult> GetAllDeliveries()
        {
            var deliveries = await taskService.GetAllDeliveriesAsync();
            if (!deliveries.Any())
            {
                return NoContent();
            }

            return Ok(deliveries);
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
