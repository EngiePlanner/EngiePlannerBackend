using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpGet("GetUsersByLeaderGroup")]
        public async Task<IActionResult> GetUsersByLeaderGroups([FromQuery] string leaderUsername)
        {
            var users = await userService.GetUsersByLeaderGroupsAsync(leaderUsername);
            if (!users.Any())
            {
                return NoContent();
            }

            return Ok(users);
        }

        [HttpGet("GetAvailabilitiesByUserUsername")]
        public async Task<IActionResult> GetAvailabilitiesByUserUsername([FromQuery] string userUsername)
        {
            var availabilities = await userService.GetAvailabilitiesByUserUsernameAsync(userUsername);
            if (!availabilities.Any())
            {
                return NoContent();
            }

            return Ok(availabilities);
        }

        [HttpGet("GettAllWeeksFromCurrentYear")]
        public IActionResult GetAllWeeksFromCurrentYear()
        {
            var weeks = userService.GetAllWeeksFromCurrentYear();
            return Ok(weeks);
        }

        [HttpGet("GetAvailabilityByFromDateAndUserUsername")]
        public async Task<IActionResult> GetAvailabilityByFromDateAndUserUsername([FromQuery] DateTime fromDate, [FromQuery] string userUsername)
        {
            var availability = await userService.GetAvailabilityByFromDateAndUserUsernameAsync(fromDate, userUsername);

            if (availability == null)
            {
                return NotFound();
            }

            return Ok(availability);
        }

        [HttpPut("UpdateAvailability")]
        public async Task<IActionResult> UpdateAvailability(AvailabilityDto availability)
        {
            try
            {
                await userService.UpdateAvailabilityAsync(availability);
                return Ok();
            }
            catch (ValidationException exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
