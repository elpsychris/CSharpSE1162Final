using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class BookStatus
    {
        public BookStatus()
        {
            Book = new HashSet<Book>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<Book> Book { get; set; }
    }
}
