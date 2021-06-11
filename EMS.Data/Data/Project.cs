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
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(4)]
        public string Size { get; set; }
        [MaxLength(50)]
        public string Color { get; set; }

        [Required]
        public decimal ListPrice { get; set; }
        [Required]
        public decimal DealerPrice { get; set; }
        [Required]
        public int TaxType { get; set; }
        [Required]
        public decimal Discount { get; set; }
        public DateTimeOffset? DiscountStartDate { get; set; }
        public DateTimeOffset? DiscountEndDate { get; set; }

        [Required]
        public int StockUnits { get; set; }
        [Required]
        public int SafetyStockLevel { get; set; }

        [Required]
        public DateTimeOffset CreatedOn { get; set; }
        [Required]
        public DateTimeOffset LastModifiedOn { get; set; }
        public string SearchTerms { get; set; }

        public byte[] Picture { get; set; }
        public byte[] Thumbnail { get; set; }

        public string BuildSearchTerms() => $"{ProjectID} {Name} {Color}".ToLower();
    }
}