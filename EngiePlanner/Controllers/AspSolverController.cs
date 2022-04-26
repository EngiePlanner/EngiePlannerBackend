using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EngiePlanner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Jwt")]
    public class AspSolverController : ControllerBase
    {
        private readonly IAspSolverService aspSolverService;

        public AspSolverController(IAspSolverService aspSolverService)
        {
            this.aspSolverService = aspSolverService;
        }

        [HttpPost("InvokeAspSolver")]
        public async Task<IActionResult> InvokeAspSolver([FromBody] List<TaskDto> tasks)
        {
            try
            {
                var scheduledTasks = await aspSolverService.InvokeAspSolver(tasks);
                return Ok(scheduledTasks);
            }
            catch (IOException exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
