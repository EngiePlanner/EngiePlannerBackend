﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BusinessObjectLayer.Dtos
{
    public class TaskDto
    {
        [JsonProperty("name")]
        public int Id { get; set; }
        [JsonIgnore]
        public string Name { get; set; }
        [JsonProperty("delivery")]
        public int DeliveryId { get; set; }
        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }
        [JsonProperty("planned_date")]
        public DateTime PlannedDate { get; set; }
        [JsonProperty("subteam")]
        public string Subteam { get; set; }
        [JsonProperty("duration")]
        public int Duration { get; set; }
        [JsonProperty("employees")]
        public List<string> Employees { get; set; }
    }
}
