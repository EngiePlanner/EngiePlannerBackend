using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjectLayer.Entities
{
    [Table("Groups")]
    public class GroupEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public virtual DepartmentEntity Department { get; set; }
    }
}
