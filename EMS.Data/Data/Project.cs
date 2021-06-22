using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Data.Data
{
    [Table("Projects")]
    public partial class Project
    {
        [MaxLength(16)]
        [Key, Column(Order = 0)]
        public long ProjectID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [Required]
        public long CustomerID { get; set; }

        [Required]
        public long EmployeeID { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        //public virtual ICollection<Employee> Employees { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }

        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        public int Progress { get; set; }
        public int Status { get; set; }

        [Required]
        public decimal Price { get; set; }
        [Required]
        public int TaxType { get; set; }

        [Required]
        public DateTimeOffset CreatedOn { get; set; }
        [Required]
        public DateTimeOffset LastModifiedOn { get; set; }
        public string SearchTerms { get; set; }

        public byte[] Picture { get; set; }
        public byte[] Thumbnail { get; set; }

        public string BuildSearchTerms() => $"{ProjectID} {Name} {CustomerID}".ToLower();
    }
}