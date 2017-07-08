using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class Category
    {
        public Category()
        {
            BookCategoryDetail = new HashSet<BookCategoryDetail>();
        }

        public int Seq { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BookCategoryDetail> BookCategoryDetail { get; set; }
    }
}
