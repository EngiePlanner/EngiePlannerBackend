using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjectLayer.Entities
{
    [Table("Availabilities")]
    public class AvailabilityEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserUsername { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public int DefaultAvailableHours { get; set; }

        [Required]
        public int UnscheduledHours { get; set; }

        public virtual UserEntity User { get; set; }
    }
}
