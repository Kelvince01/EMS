using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Data.Data
{
    [Table("Employees")]
    public partial class Employee
    {
        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public long EmployeeID { get; set; }

        [MaxLength(8)]
        public string Title { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string MiddleName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [MaxLength(10)]
        public string Suffix { get; set; }
        [MaxLength(1)]
        public string Gender { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmailAddress { get; set; }
        [Required]
        [MaxLength(120)]
        public string AddressLine1 { get; set; }
        [MaxLength(120)]
        public string AddressLine2 { get; set; }
        [Required]
        [MaxLength(30)]
        public string City { get; set; }
        [Required]
        [MaxLength(50)]
        public string Region { get; set; }
        [Required]
        [MaxLength(2)]
        public string CountryCode { get; set; }
        [Required]
        [MaxLength(15)]
        public string PostalCode { get; set; }
        [MaxLength(20)]
        public string Phone { get; set; }

        public DateTimeOffset? BirthDate { get; set; }
        [MaxLength(40)]
        public string Education { get; set; }
        [MaxLength(100)]
        public string Occupation { get; set; }
        public decimal? Salary { get; set; }
        [MaxLength(1)]
        public string MaritalStatus { get; set; }
        public int? TotalChildren { get; set; }
        public int? ChildrenAtHome { get; set; }
        public bool? IsHouseOwner { get; set; }
        public int? NumberCarsOwned { get; set; }

        [Required]
        public DateTimeOffset CreatedOn { get; set; }
        [Required]
        public DateTimeOffset? LastModifiedOn { get; set; }
        public string SearchTerms { get; set; }

        public byte[] Picture { get; set; }
        public byte[] Thumbnail { get; set; }

        public virtual ICollection<Project> Projects { get; set; }

        public string BuildSearchTerms() => $"{EmployeeID} {FirstName} {LastName} {EmailAddress} {AddressLine1}".ToLower();
    }
}