using System;
using EMS.ViewModels.Infrastructure.ViewModels;

namespace EMS.ViewModels.Models
{
    public class CompanyModel : ObservableObject
    {
        static public CompanyModel CreateEmpty() => new CompanyModel { CompanyID = -1, IsEmpty = true };

        public long CompanyID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? LastModifiedOn { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Contact { get; set; }
        public string ABN { get; set; }
        public string ACN { get; set; }
        public byte[] Picture { get; set; }
        public object PictureSource { get; set; }

        public bool IsNew => CompanyID <= 0;
        public string Initials => String.Format("{0}", $"{Name} "[0]).Trim().ToUpper();
    }
}