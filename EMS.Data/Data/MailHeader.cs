using MailKit;

namespace EMS.Data.Data
{
    public class MailHeader
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }
        public UniqueId UniqueId { get; set; }
    }
}