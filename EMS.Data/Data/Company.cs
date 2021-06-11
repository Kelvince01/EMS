using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Data.Data
{
    [Table("Company")]
    public partial class Company
    {
        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public long CompanyID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [MaxLength(400)]
        public string Description { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public DateTimeOffset CreatedOn { get; set; }
        [Required]
        public DateTimeOffset? LastModifiedOn { get; set; }
        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
        [Required]
        [MaxLength(50)]
        public string Email { get; set; }
        [Required]
        public string Website { get; set; }
        [Required]
        public string Contact { get; set; }
        [Required]
        public string ABN { get; set; }
        [Required]
        public string ACN { get; set; }
        [Required]
        public byte[] Picture { get; set; }
        public string SearchTerms { get; set; }

        public string BuildSearchTerms() => $"{CompanyID} {Name} {Address} {Contact}".ToLower();
    }
}