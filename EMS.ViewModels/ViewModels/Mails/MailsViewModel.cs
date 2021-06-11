using System;
using System.Linq.Expressions;
using EMS.Data.Data;
using EMS.ViewModels.Infrastructure.Services;
using EMS.ViewModels.Infrastructure.ViewModels;

namespace EMS.ViewModels.ViewModels.Mails
{
    #region MailListArgs
    public class MailListArgs
    {
        static public MailListArgs CreateEmpty() => new MailListArgs { IsEmpty = true };

        public MailListArgs()
        {
            OrderBy = r => r.MailID;
        }

        public bool IsEmpty { get; set; }

        public string Query { get; set; }

        public Expression<Func<Mail, object>> OrderBy { get; set; }
        public Expression<Func<Mail, object>> OrderByDesc { get; set; }
    }
    #endregion

    public class MailsViewModel : ViewModelBase
    {
        public MailsViewModel(ICommonServices commonServices) : base(commonServices)
        {
        }
    }
}