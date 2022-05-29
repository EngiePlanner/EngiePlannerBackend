using AutoMapper;
using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class AspSolverService : IAspSolverService
    {
        private readonly ITaskRepository taskRepository;
        private readonly IAvailabilityRepository availabilityRepository;
        private readonly IMapper mapper;
        private readonly ILogger<AspSolverService> logger;
        private static readonly string aspDataDirectory = AppDomain.CurrentDomain.GetData("AspDataDirectory").ToString();
        //private static readonly string aspDataDirectory = @"E:\Facultate\EngiePlannerAPI\EngiePlanner\AspData";

        public AspSolverService(
            ITaskRepository taskRepository, 
            IAvailabilityRepository availabilityRepository,
            IMapper mapper,
            ILogger<AspSolverService> logger)
        {
            this.taskRepository = taskRepository;
            this.availabilityRepository = availabilityRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<List<TaskDto>> InvokeAspSolver(List<TaskDto> tasks)
        {
            var availabilities = new List<AvailabilityDto>();
            var users = tasks.Select(x => x.ResponsibleUsername).Distinct().ToList();

            foreach (var user in users)
            {
                availabilities.AddRange(
                    (await availabilityRepository.GetAvailabilitiesByUserUsernameAsync(user))
                    .Select(mapper.Map<AvailabilityEntity, AvailabilityDto>));
            }

            CreateAvailabilityJsonFile(availabilities);
            CreateTaskJsonFile(tasks);
            CallPythonScript("create_encoding.py");
            CallPythonScript("run_clingo.py");
            var result = ReadJsonResult();
            return SetStartAndEndDateOnTasks(tasks, result);
        }

        private void CreateAvailabilityJsonFile(List<AvailabilityDto> availabilities)
        {
            availabilities.ForEach(x => x.UserUsername = x.UserUsername.ToLower());
            var availabilitiesJson = availabilities.Select(mapper.Map<AvailabilityDto, AvailabilityJsonDto>).ToList();

            var jsonSerializerOptions = new JsonSerializerSettings
            {
                DateFormatString = "dd.MM.yyyy",
                Formatting = Formatting.Indented,
            };
            string availabilityJson = JsonConvert.SerializeObject(availabilitiesJson, jsonSerializerOptions);

            var path = aspDataDirectory + "\\availability.json";
            if (!File.Exists(path))
            {
                File.CreateText(path);
            }
            File.WriteAllText(path, availabilityJson);
        }

        private void CreateTaskJsonFile(List<TaskDto> tasks)
        {
            var tasksJson = new List<TaskJsonDto>();
            foreach (var task in tasks)
            {
                var taskJson = new TaskJsonDto
                {
                    Id = task.Id,
                    Name = task.Name,
                    StartDate = task.AvailabilityDate,
                    PlannedDate = task.PlannedDate,
                    Subteam = task.Subteam,
                    Duration = task.Duration,
                    Employees = new List<string> { task.ResponsibleUsername.ToLower() },
                    Predecessors = task.Predecessors.Select(x => x.Id).ToList()
                };

                tasksJson.Add(taskJson);
            }

            var tasksDictionary = new Dictionary<string, List<TaskJsonDto>>
            {
                { "tasks", tasksJson }
            };
            
            var jsonSerializerOptions = new JsonSerializerSettings
            {
                DateFormatString = "dd.MM.yyyy",
                Formatting = Formatting.Indented,
            };
            string availabilityJson = JsonConvert.SerializeObject(tasksDictionary, jsonSerializerOptions);

            var path = aspDataDirectory + "\\tasks.json";
            if (!File.Exists(path))
            {
                File.CreateText(path);
            }
            File.WriteAllText(path, availabilityJson);
        }

        private void CallPythonScript(string script)
        {
            var cmd = aspDataDirectory + "\\" + script;
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python";
            start.Arguments = string.Format("\"{0}\"", cmd);
            start.UseShellExecute = false;
            start.CreateNoWindow = true; 
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            try 
            {
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string stderr = process.StandardError.ReadToEnd();

                        /*if (stderr.Length != 0 && !stderr.Equals(Constants.ClingoWarnings))
                        {
                            Debug.WriteLine(stderr);
                            throw new IOException(stderr);
                        }*/

                        string result = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error is: " + ex.Message);
            }
        }

        private static List<AspResultDto> ReadJsonResult()
        {
            var path = aspDataDirectory + "\\output.json";
            var json = File.ReadAllText(path);
            var jsonSerializerOptions = new JsonSerializerSettings
            {
                DateFormatString = "dd.MM.yyyy",
            };
            var aspResults = JsonConvert.DeserializeObject<List<AspResultDto>>(json, jsonSerializerOptions);
            return aspResults;
        }

        private static List<TaskDto> SetStartAndEndDateOnTasks(List<TaskDto> tasks, List<AspResultDto> result)
        {
            foreach (var task in tasks)
            {
                var item = result.FirstOrDefault(x => x.Task == task.Id);
                task.StartDate = item.Start;
                task.EndDate = item.Finish;
            }

            return tasks;
        }
    }
}
