using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.Models
{
    public class UserErrorObject
    {
        public string IdDuplicated { get; set; }
        public string IdNotFound { get; set; }
        public bool hasError { get; set; }

        public UserErrorObject()
        {
            IdDuplicated = "";
            IdNotFound = "";
            hasError = false;
        }

        public void setErrorMessageIfIdDuplicated(string MID)
        {
            List<Account> list = null;
            using (var context = new OnlineLibraryContext())
            {
                list = context.Account.ToList();
                foreach(Account acc in list)
                {
                    if (acc.Id.Equals(MID))
                    {
                        IdDuplicated = "The user " + MID + " has already existed!";
                        hasError = true;
                        return;
                    }
                }
            }
        }

        public void setErrorMessageIfIdNotFound(string MID)
        {
            List<Account> list = null;
            using(var context = new OnlineLibraryContext())
            {
                list = context.Account.Where(m => m.Id.Equals(MID)).ToList();
                if(list.Count == 0)
                {
                    IdNotFound = "The user " + MID + " cannot be found!";
                    hasError = true;
                    return;
                }
            }
        }
    }
}
