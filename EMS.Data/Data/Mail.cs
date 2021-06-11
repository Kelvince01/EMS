using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EMS.Data.Data;

namespace EMS.Data.Data
{
    [Table("Mails")]
    public partial class Mail
    {
        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public long MailID { get; set; }
    }
}