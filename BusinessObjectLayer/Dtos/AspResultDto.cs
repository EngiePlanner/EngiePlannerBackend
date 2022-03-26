using Newtonsoft.Json;
using System;

namespace BusinessObjectLayer.Dtos
{
    public class AspResultDto
    {
        [JsonProperty("task")]
        public int Task { get; set; }
        [JsonProperty("start")]
        public DateTime Start { get; set; }
        [JsonProperty("finish")]
        public DateTime Finish { get; set; }
    }
}
