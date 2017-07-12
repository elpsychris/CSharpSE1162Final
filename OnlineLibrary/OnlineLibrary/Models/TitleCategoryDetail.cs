using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class TitleCategoryDetail
    {
        public int Seq { get; set; }
        public int? TitleId { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Title Title { get; set; }
    }
}
