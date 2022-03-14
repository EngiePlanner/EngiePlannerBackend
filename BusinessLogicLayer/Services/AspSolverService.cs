using AutoMapper;
using BusinessLogicLayer.Interfaces;
using BusinessObjectLayer.Dtos;
using BusinessObjectLayer.Entities;
using DataAccessLayer.Interfaces;
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
        private static readonly string aspDataDirectory = AppDomain.CurrentDomain.GetData("AspDataDirectory").ToString();

        public AspSolverService(
            ITaskRepository taskRepository, 
            IAvailabilityRepository availabilityRepository,
            IMapper mapper)
        {
            this.taskRepository = taskRepository;
            this.availabilityRepository = availabilityRepository;
            this.mapper = mapper;
        }

        public async Task InvokeAspSolver(List<TaskDto> tasks)
        {
            var availabilities = new List<AvailabilityDto>();
            var users = tasks.Select(x => x.Employees.FirstOrDefault()).Distinct().ToList();

            foreach (var user in users)
            {
                availabilities.AddRange(
                    (await availabilityRepository.GetAvailabilitiesByUserUsernameAsync(user))
                    .Select(mapper.Map<AvailabilityEntity, AvailabilityDto>));
            }

            CreateAvailabilityJsonFile(availabilities);
            CreateTaskJsonFile(tasks);
            CallPythonScript("pot_plot_json_io.py");
        }

        private static void CreateAvailabilityJsonFile(List<AvailabilityDto> availabilities)
        {
            availabilities.ForEach(x => x.UserUsername = x.UserUsername.ToLower());

            var jsonSerializerOptions = new JsonSerializerSettings
            {
                DateFormatString = "dd.MM.yyyy",
                Formatting = Formatting.Indented,
            };
            string availabilityJson = JsonConvert.SerializeObject(availabilities, jsonSerializerOptions);

            var path = aspDataDirectory + "\\availability.json";
            if (!File.Exists(path))
            {
                File.CreateText(path);
            }
            File.WriteAllText(path, availabilityJson);
        }

        private static void CreateTaskJsonFile(List<TaskDto> tasks)
        {
            foreach (var task in tasks)
            {
                task.Employees = task.Employees.Select(x => x.ToLower()).ToList();
            }
            
            var tasksDictionary = new Dictionary<string, List<TaskDto>>
            {
                { "tasks", tasks }
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

        private static void CallPythonScript(string script)
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
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd(); 

                    if (stderr.Length != 0)
                    {
                        throw new IOException(stderr);
                    }

                    string result = reader.ReadToEnd(); 
                }
            }
        }
    }
}
