using BusinessObjectLayer.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjectLayer.Entities
{
    [Table("Users")]
    public class UserEntity
    {
        [Key]
        [MaxLength(10)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public RoleType RoleType { get; set; }

        [ForeignKey("Leader")]
        public string LeaderUsername { get; set; }

        public virtual UserEntity Leader { get; set; }

        public virtual ICollection<TaskEntity> Tasks { get; set; }

        public virtual ICollection<UserDepartmentMapping> UserDepartments { get; set; }

        public virtual ICollection<UserGroupMapping> UserGroups { get; set; }

        public virtual ICollection<AvailabilityEntity> Availabilities { get; set; }
    }
}

