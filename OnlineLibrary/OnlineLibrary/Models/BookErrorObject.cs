using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.Models
{
    public class BookErrorObject
    {
        public string IdDuplicated { get; set; }
        public string IdNotFound { get; set; }
        public bool hasError { get; set; }

        public BookErrorObject()
        {
            IdDuplicated = "";
            IdNotFound = "";
            hasError = false;
        }

        public void setErrorMessageIfIdDuplicated(string BID)
        {
            List<Book> list = null;
            using (var context = new OnlineLibraryContext())
            {
                list = context.Book.ToList();
                foreach (Book b in list)
                {
                    if (b.Id.Equals(BID))
                    {
                        IdDuplicated = "The book " + BID + " has already existed!";
                        hasError = true;
                        return;
                    }
                }
            }
        }

        public void setErrorMessageIfIdNotFound(string BID)
        {
            List<Book> list = null;
            using (var context = new OnlineLibraryContext())
            {
                list = context.Book.Where(m => m.Id.Equals(BID)).ToList();
                if (list.Count == 0)
                {
                    IdNotFound = "The book " + BID + " cannot be found!";
                    hasError = true;
                    return;
                }
            }
        }
    }
}
