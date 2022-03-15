﻿using Newtonsoft.Json;
using System;

namespace BusinessObjectLayer.Dtos
{
    public class AvailabilityDto
    {
        public int Id { get; set; }
        public string UserUsername { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int AvailableHours { get; set; }
    }
}
