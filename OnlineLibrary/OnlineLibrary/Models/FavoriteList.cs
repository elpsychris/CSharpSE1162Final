using System;
using System.Collections.Generic;

namespace OnlineLibrary.Models
{
    public partial class FavoriteList
    {
        public int Seq { get; set; }
        public int? TitleId { get; set; }
        public int? MemberId { get; set; }

        public virtual Account Member { get; set; }
        public virtual Title Title { get; set; }
    }
}
