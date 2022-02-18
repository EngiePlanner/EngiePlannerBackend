using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjectLayer.Entities
{
    [Table("UserGroupMappings")]
    public class UserGroupMapping
    {
        [Required]
        [ForeignKey("User")]
        [MaxLength(10)]
        public string UserUsername { get; set; }

        [Required]
        [ForeignKey("Group")]
        public int GroupId { get; set; }

        public virtual UserEntity User { get; set; }

        public virtual GroupEntity Group { get; set; }
    }
}
