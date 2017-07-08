using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class ReturnType
    {
        public ReturnType()
        {
            Book = new HashSet<Book>();
        }

        public int Seq { get; set; }
        public int? NoOfDay { get; set; }

        public virtual ICollection<Book> Book { get; set; }
    }
}
