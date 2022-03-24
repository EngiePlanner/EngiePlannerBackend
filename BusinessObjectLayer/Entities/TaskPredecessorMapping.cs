using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.Entities
{
    [Table("TaskPredecessorsMappings")]
    public class TaskPredecessorMapping
    {
        public int TaskId { get; set; }
        
        public int PredecessorId { get; set; }

        public TaskEntity Task { get; set; }

        public TaskEntity Predecessor { get; set; }
    }
}
