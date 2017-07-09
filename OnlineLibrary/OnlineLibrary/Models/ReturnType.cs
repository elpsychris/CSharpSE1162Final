using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class ReturnType
    {
        public ReturnType()
        {
            Title = new HashSet<Title>();
        }

        public int Seq { get; set; }
        public int? NoOfDay { get; set; }

        public virtual ICollection<Title> Title { get; set; }
    }
}
