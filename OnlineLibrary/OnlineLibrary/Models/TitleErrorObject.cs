using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.Models
{
    public class TitleErrorObject
    {
        public string IdNotFound { get; set; }
        public bool hasError { get; set; }

        public TitleErrorObject()
        {
            IdNotFound = "";
            hasError = false;
        }

        public void setErrorMessageIfIdNotFound(string TID)
        {
            List<Title> list = null;
            using (var context = new OnlineLibraryContext())
            {
                list = context.Title.Where(t => t.Seq == Int32.Parse(TID)).ToList();
                if (list.Count == 0)
                {
                    IdNotFound = "The title " + TID + " cannot be found!";
                    hasError = true;
                    return;
                }
            }
        }
    }
}
