using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("AddAvailability")]
        public async Task<IActionResult> CreateAvailability(AvailabilityDto availability)
        {
            await userService.CreateAvailabilityAsync(availability);
            return Ok();
        }
    }
}
