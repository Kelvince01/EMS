using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Data.Data
{
    [Table("ProjectStatus")]
    public class ProjectStatus
    {
        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public int Status { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}