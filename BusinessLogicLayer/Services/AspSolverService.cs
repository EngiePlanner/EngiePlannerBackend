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
        //private static readonly string aspDataDirectory = @"E:\Licenta\Deploy\Backend\AspData";
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
            logger.LogInformation("InvokeASPSolver");

            CreateAvailabilityJsonFile(availabilities);
            CreateTaskJsonFile(tasks);
            CallPythonScript("create_encoding.py");
            CallPythonScript("run_clingo.py");
            var result = ReadJsonResult();
            return SetStartAndEndDateOnTasks(tasks, result);
        }

        private void CreateAvailabilityJsonFile(List<AvailabilityDto> availabilities)
        {
            try
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
                logger.LogInformation("AvailabilityPath:  " + path);
            }
            catch(Exception ex)
            {
                logger.LogError("AvailabilityError:  " + ex.Message);
            }
        }

        private void CreateTaskJsonFile(List<TaskDto> tasks)
        {
            try
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
                logger.LogInformation("TasksPath:  " + path);
            }
            catch(Exception ex)
            {
                logger.LogError("TasksError:  " + ex.Message);
            }
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
            
            using (Process process = Process.Start(start))
            {
                logger.LogError("Process: " + process);
                using (StreamReader reader = process.StandardOutput)
                {
                    logger.LogError("Reader: " + reader);
                    string stderr = process.StandardError.ReadToEnd();
                    logger.LogError("StdErr: " + stderr);
                    /*if (stderr.Length != 0 && !stderr.Equals(Constants.ClingoWarnings))
                    {
                        Debug.WriteLine(stderr);
                        throw new IOException(stderr);
                    }*/

                    string result = reader.ReadToEnd();
                    logger.LogError("Result: " + result);
                }
            }
            
            logger.LogError("Error at " + script);
            logger.LogError("Path is: " + cmd);
            logger.LogError("Start argumets: " + start.Arguments);
            logger.LogError("Start fileName: " + start.FileName);
            
        }

        private List<AspResultDto> ReadJsonResult()
        {
            try
            {
                var path = aspDataDirectory + "\\output.json";
                var json = File.ReadAllText(path);
                var jsonSerializerOptions = new JsonSerializerSettings
                {
                    DateFormatString = "dd.MM.yyyy",
                };
                var aspResults = JsonConvert.DeserializeObject<List<AspResultDto>>(json, jsonSerializerOptions);
                logger.LogInformation("ReadJsonResultPath: " + path);
                return aspResults;
            }
            catch(Exception ex)
            {
                logger.LogError("ReadJsonResult: " + ex.Message);
                return new List<AspResultDto>();
            }
        }

        private List<TaskDto> SetStartAndEndDateOnTasks(List<TaskDto> tasks, List<AspResultDto> result)
        {
            try
            {
                foreach (var task in tasks)
                {
                    var item = result.FirstOrDefault(x => x.Task == task.Id);
                    task.StartDate = item.Start;
                    task.EndDate = item.Finish;
                }
                logger.LogInformation("SetStartAndEnd: AllGood");
                return tasks;
            }
            catch (Exception ex)
            {
                logger.LogError("SetStartAndEndError: " + ex.Message);
                return new List<TaskDto>();
            }
        }
    }
}
