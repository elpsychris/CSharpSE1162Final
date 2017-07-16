using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.Models
{
    public class RegistrationMemberBookErrorObject
    {
        public string IdFoundButDeactivated { get; set; } //IFBD
        public string IdFoundButAlreadyRegistered { get; set; } //IFBAR
        public string IdNotFound { get; set; } //INF
        public bool hasError { get; set; }

        public RegistrationMemberBookErrorObject()
        {
            this.IdFoundButDeactivated = "";
            this.IdFoundButAlreadyRegistered = "";
            this.IdNotFound = "";
            this.hasError = false;
        }

        public void setMessageIfIFBD(string MID)
        {
            Account account = null;
            using (var context = new OnlineLibraryContext())
            {
                account = context.Account.Where(a => a.Id.Equals(MID)).SingleOrDefault();
                if (account == null)
                {
                    setMessageIfINF(MID);
                    return;
                }
                if (!account.Status.Value)
                {
                    IdFoundButDeactivated = "The user " + MID + " has been deactivated, please activate to register!";
                    hasError = true;
                }
            }
        }

        public void setMessageIfIFBAR(string MID, string BID)
        {
            List<ActivityHistory> ahs = null;
            using (var context = new OnlineLibraryContext())
            {
                ahs = context.ActivityHistory.Where(ah => ah.Bid.Equals(BID) && ah.Mid.Equals(MID) && ah.ReturnDate == null).ToList();
                if (ahs.Count != 0)
                {
                    IdFoundButAlreadyRegistered = "The user " + MID + " is currently borrowing this book!";
                    hasError = true;
                }
            }
        }

        public void setMessageIfINF(string MID)
        {
            Account acc = null;
            using (var context = new OnlineLibraryContext())
            {
                acc = context.Account.Where(a => a.Id.Equals(MID)).SingleOrDefault();
                if(acc == null)
                {
                    IdNotFound = "The user " + MID + " is not a member on the system!";
                    hasError = true;
                }
            }
        }
    }
}
