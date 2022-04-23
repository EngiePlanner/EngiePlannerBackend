using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjectLayer.Entities
{
    [Table("Tasks")]
    public class TaskEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime AvailabilityDate { get; set; }

        [Required]
        public DateTime PlannedDate { get; set; }

        public string Subteam { get; set; }

        [Required]
        public int Duration { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? StartDate { get; set; }

        public virtual ICollection<UserTaskMapping> UserTasks { get; set; }
        public virtual ICollection<TaskPredecessorMapping> Predecessors { get; set; }
        public virtual ICollection<TaskPredecessorMapping> Tasks { get; set; }
    }
}
