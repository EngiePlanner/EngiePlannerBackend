using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
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
                return NotFound();
            }

            return Ok(tasks);
        }

        [HttpPost("task")]
        public async Task<IActionResult> CreateTaskAsync([FromBody] TaskDto task)
        {
            await taskService.CreateTaskAsync(task);
            return Ok();
        }
    }
}
