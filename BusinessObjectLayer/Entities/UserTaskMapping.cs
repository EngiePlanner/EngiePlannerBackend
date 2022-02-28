﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjectLayer.Entities
{
    [Table("UserTaskMappings")]
    public class UserTaskMapping
    {
        [Required]
        [ForeignKey("User")]
        [MaxLength(10)]
        public string UserUsername { get; set; }

        [Required]
        [ForeignKey("Task")]
        public int TaskId { get; set; }

        public virtual UserEntity User { get; set; }
        public virtual TaskEntity Task { get; set; }
    }
}
