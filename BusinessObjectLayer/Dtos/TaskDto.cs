﻿using System;
using System.Collections.Generic;

namespace BusinessObjectLayer.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DeliveryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedDate { get; set; }
        public string Subteam { get; set; }
        public int Duration { get; set; }
        public List<string> Employees { get; set; }
    }
}
