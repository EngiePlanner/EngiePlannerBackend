using Newtonsoft.Json;
using System;

namespace BusinessObjectLayer.Dtos
{
    public class AvailabilityJsonDto
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string UserUsername { get; set; }
        [JsonProperty("fromDate")]
        public DateTime FromDate { get; set; }
        [JsonProperty("toDate")]
        public DateTime ToDate { get; set; }
        [JsonProperty("availableHours")]
        public int UnscheduledHours { get; set; }
    }
}
