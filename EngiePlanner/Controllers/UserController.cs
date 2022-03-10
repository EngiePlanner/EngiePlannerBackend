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
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userService.GetAllUsersAsync();
            if (!users.Any())
            {
                return NoContent();
            }

            return Ok(users);
        }

        [HttpPost("CreateAvailability")]
        public async Task<IActionResult> CreateAvailability(AvailabilityDto availability)
        {
            await userService.CreateAvailabilityAsync(availability);
            return Ok();
        }
    }
}
