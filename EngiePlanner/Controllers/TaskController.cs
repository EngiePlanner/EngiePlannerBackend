﻿using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Helpers;
using BusinessObjectLayer.Validators;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EngiePlanner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Jwt")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService taskService;
        private readonly IUserService userService;
        private readonly IConverter converter;
        private ILogger<TaskController> logger;

        public TaskController(ITaskService taskService, IUserService userService, IConverter converter, ILogger<TaskController> logger)
        {
            this.taskService = taskService;
            this.userService = userService;
            this.converter = converter;
            this.logger = logger;
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
            var tasks = await taskService.GetTasksByOwnerUsernameAsync(ownerUsername);

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

        [HttpPut("UpdateTasksAfterSchedule")]
        public async Task<IActionResult> UpdateTasksAfterSchedule([FromBody] List<TaskDto> tasks)
        {
            try
            {
                await taskService.UpdateTaskRangeAsync(tasks);
                await userService.UpdateUnscheduledHoursAsync(tasks);
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

        [HttpPost("CreateScheduledTasksPdf")]
        public async Task<IActionResult> CreatePDF([FromBody] List<TaskDto> tasks)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Left = 0, Bottom = 3, Right = 0, Top = 10 },
                DocumentTitle = "Scheduled Tasks",
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = PdfTemplate.GetHTMLString(tasks),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { HtmUrl = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "footer.html") }
            };

            var pdf = new HtmlToPdfDocument
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            try
            {
                var file = converter.Convert(pdf);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                logger.LogError("PDF Error: " + ex.Message);
                return BadRequest();
            }
        }
    }
}
