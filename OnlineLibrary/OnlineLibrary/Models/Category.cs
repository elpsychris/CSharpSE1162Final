using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class Category
    {
        public Category()
        {
            TitleCategoryDetail = new HashSet<TitleCategoryDetail>();
        }

        public int Seq { get; set; }
        public string Name { get; set; }

        public virtual ICollection<TitleCategoryDetail> TitleCategoryDetail { get; set; }
    }
}
