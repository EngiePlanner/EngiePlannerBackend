using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjectLayer.Entities
{
    [Table("UserDepartmentMappings")]
    public class UserDepartmentMapping
    {
        [Required]
        [ForeignKey("User")]
        [MaxLength(10)]
        public string UserUsername { get; set; }

        [Required]
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public virtual UserEntity User { get; set; }

        public virtual DepartmentEntity Department { get; set; }
    }
}
