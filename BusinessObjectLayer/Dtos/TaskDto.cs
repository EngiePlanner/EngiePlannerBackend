using System;
using System.Collections.Generic;

namespace BusinessObjectLayer.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime AvailabilityDate { get; set; }
        public DateTime PlannedDate { get; set; }
        public string Subteam { get; set; }
        public int Duration { get; set; }
        public List<TaskDto> Predecessors { get; set; }
        public string ResponsibleUsername { get; set; }
        public string ResponsibleDisplayName { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string OwnerUsername { get; set; }
    }
}
