using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class BookCategoryDetail
    {
        public int Seq { get; set; }
        public int? BookId { get; set; }
        public int? CategoryId { get; set; }

        public virtual Book Book { get; set; }
        public virtual Category Category { get; set; }
    }
}
